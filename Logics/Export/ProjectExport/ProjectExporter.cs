using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using Logics.Export.ModelExport.Extractors;
using Logics.Export.ModelExport.Extractors.Implementations;
using Logics.Export.Wraps.Implementations;
using Logics.Export.Wraps.Interfaces;
using PluginUtil;
using PluginUtil.Loger;
using Newtonsoft.Json;

namespace Logics.Export.ModelExport
{
	public class ProjectExporter
	{
		private readonly Document _doc;
		private Dictionary<Type, IExtractor> _extracters;
		public ProjectExporter(Document doc)
		{
			if (doc.IsFamilyDocument)
			{
				throw new Exception($"Document {doc.Title} is not project document");
			}

			_doc = doc;
		}

		public void ExportToJson(string json, ProjectDocumentWrap prDocWrap)
        {
			prDocWrap.Name = _doc.Title;
			string dataJson = JsonConvert.SerializeObject(prDocWrap);

			TextWriter tw = new StreamWriter(json);
			using (tw)
			{
				tw.Write(dataJson);
			}
		}

		public ProjectDocumentWrap GetProjDocWrap()
		{
			var docWrap = new ProjectDocumentWrap();


			var props = docWrap.GetType()
							   .GetProperties()
							   .Where(x => x.PropertyType.IsDictionary())
							   .Where(x => x.PropertyType.GetGenericArguments()
											.LastOrDefault()
											.IsSubclassOf(typeof(AbstractElementData)));

			_extracters = CollectExtracters();
			foreach (var prop in props)
			{
				ExtractDataFromProject(prop.PropertyType.GetGenericArguments().Last())
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

			if (_doc.IsFamilyDocument == true)
            {
				return null;
            }
			else
            {
				return (IExtractor)constr.Invoke(new object[] { _doc });
			}
		}

		private Result<IDictionary> ExtractDataFromProject(Type last)
		{
			if (_extracters.ContainsKey(last))
			{
				return _extracters[last].Extract().ToResult();
			}

			return Result.Failure<IDictionary>($"Extractor for type {last.Name} not founded ");
		}
	}
}