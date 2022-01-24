using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Logics.FamilyExport.ModelExport.Extractors;
using Logics.FamilyExport.ModelExport.Extractors.Implementations;
using Logics.FamilyExport.Wraps.Implementations;
using Logics.FamilyExport.Wraps.Interfaces;
using PluginUtil;
using PluginUtil.Loger;
using r = Autodesk.Revit.UI;

namespace Logics.FamilyExport.ModelExport
{
	public class FamilyExporter
	{
		private readonly Document _doc;
		private Dictionary<Type, IExtractor> _extracters;
		public FamilyExporter(Document doc)
		{
			if (!doc.IsFamilyDocument)
			{
				throw new Exception($"Document {doc.Title} is not family document");
			}

			_doc = doc;
		}


		public FamilyDocumentWrap Export()
		{
			var docWrap = new FamilyDocumentWrap();


			var props = docWrap.GetType()
							   .GetProperties()
							   .Where(x => x.PropertyType.IsDictionary())
							   .Where(x => x.PropertyType.GetGenericArguments()
											.LastOrDefault()
											.IsSubclassOf(typeof(AbstractElementData)));

			_extracters = CollectExtracters();
			foreach (var prop in props)
			{
				ExtractDataFromFamily(prop.PropertyType.GetGenericArguments().Last())
					.OnSuccessTry(x => prop.SetValue(docWrap, x))
					.OnFailure(x => new Exception(x).LogError());
			}
			
			return docWrap;
		}

		private Dictionary<Type, IExtractor> CollectExtracters()
		{
			var extrs = new Dictionary<Type, IExtractor>();
			var extractors = Assembly.GetExecutingAssembly()
									 .GetTypes()
									 .Where(x => x.HaveInterface(typeof(IExtractor)))
									 .Where(x => !x.IsAbstract)
									 .ToList();

			foreach (var type in extractors)
			{
				var genType = type.BaseType.GetGenericArguments().FirstOrDefault();
				var ext = CreateInstance(type);
				extrs[genType] = ext;
			}


			return extrs;
		}

		private IExtractor CreateInstance(Type type)
		{

			type.GetConstructors().Select(x => x.GetParameters().Length).Debug();
			var constr = type.GetConstructors()
							 .Where(x => x.GetParameters().Length == 1)
							 .FirstOrDefault(x => x.GetParameters().FirstOrDefault().ParameterType == typeof(Document));

			return (IExtractor)constr.Invoke(new object[] { _doc });
		}

		private Result<IDictionary> ExtractDataFromFamily(Type last)
		{
			if (_extracters.ContainsKey(last))
			{
				return _extracters[last].Extract().ToResult();
			}

			return Result.Failure<IDictionary>($"Extractor for type {last.Name} not founded ");
		}
	}
}