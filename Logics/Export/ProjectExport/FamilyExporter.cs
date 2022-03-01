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
using ui = Autodesk.Revit.UI;
using Logics.Geometry;

namespace Logics.Export.ModelExport
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

		public void ExportToJson(string json, FamilyDocumentWrap famDocWrap)
        {
			famDocWrap.Name = _doc.Title;
			string dataJson = JsonConvert.SerializeObject(famDocWrap);

			TextWriter tw = new StreamWriter(json);
			using (tw)
			{
				tw.Write(dataJson);
			}
		}

		public FamilyDocumentWrap GetFamDocWrap()
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
	public static class ExportMethods
    {
		public static double[] ToJsonDoubles(this XYZ xyz)
		{
			double[] a = { xyz.X, xyz.Y, xyz.Z };
			return a;
		}

		public static double[] ToJsonDoubles(this double X)
		{
			double[] a = { X };
			return a;
		}

		public static double[] ToJsonDoubles(this Curve curve)
		{
			if (!curve.IsCyclic && curve.IsBound)
			{
				double[] a = curve.GetEndPoint(0).ToJsonDoubles().
					  Concat(curve.GetEndPoint(1).ToJsonDoubles()).ToArray();
				return a;
			}
			else
			{
				Arc arc = curve as Arc;
				double[] a = arc.GetEndPoint(0).ToJsonDoubles().
					  Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
					  Concat(GeometryOperations.GetPointOnArc(arc).ToJsonDoubles()).ToArray();
				return a;
			}
		}
	}
}