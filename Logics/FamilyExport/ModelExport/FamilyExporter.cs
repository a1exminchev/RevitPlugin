using System;
using System.IO;
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
using Newtonsoft.Json;

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

		public void ExportToJson(string json, FamilyDocumentWrap famDocWrap)
        {
			string dataJson = "{\n" + JsonConvert.SerializeObject("Extrusions") + ":\n{\n";
			foreach (var pair in famDocWrap.Extrusions)
			{
				dataJson += pair.Value.ExtrusionWrapProperties.ToJsonString();
			}
			if (famDocWrap.Extrusions.Count != 0)
			{
				dataJson = dataJson.Remove(dataJson.Length - 2, 1) + "},\n";
			}
			else
			{
				dataJson += "\n},\n";
			}
			dataJson += JsonConvert.SerializeObject("Revolutions") + ":\n{\n";
			foreach (var pair in famDocWrap.Revolutions)
			{
				dataJson += pair.Value.RevolutionWrapProperties.ToJsonString();
			}
			if (famDocWrap.Revolutions.Count != 0)
			{
				dataJson = dataJson.Remove(dataJson.Length - 2, 1) + "},\n";
			}
			else
			{
				dataJson += "\n},\n";
			}
			dataJson += JsonConvert.SerializeObject("Blends") + ":\n{\n";
			foreach (var pair in famDocWrap.Blends)
			{
				dataJson += pair.Value.BlendWrapProperties.ToJsonString();
			}
			if (famDocWrap.Blends.Count != 0)
			{
				dataJson = dataJson.Remove(dataJson.Length - 2, 1) + "}";
			}
			else
			{
				dataJson += "\n}";
			}
			dataJson += "\n}";
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

		public static string ToJsonString(this ExtrusionWrapParameters ExtrusionWrapProperties)
		{
			string dataJson = "";
			dataJson += JsonConvert.SerializeObject("Extrusion" + ExtrusionWrapProperties.Id.ToString()) + ":\n{\n";

			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.isSolid.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.isSolid.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.StartOffset.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.StartOffset.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.EndOffset.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(ExtrusionWrapProperties.EndOffset.First().Value) + ",\n";

			foreach (var Dicts in ExtrusionWrapProperties.SketchPlane)
			{
				dataJson += JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
                {
					foreach (var pair in dict)
                    {
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "},";
			}
			foreach (var Dicts in ExtrusionWrapProperties.CurveArrArray)
			{
				dataJson += "\n" + JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
                {
					foreach (var pair in dict)
                    {
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "\n}";
			}

			dataJson = dataJson.Remove(dataJson.Length - 3, 1);
			dataJson += "\n},\n";
			return dataJson;
		}

		public static string ToJsonString(this RevolutionWrapParameters RevolutionWrapProperties)
		{
			string dataJson = "";
			dataJson += JsonConvert.SerializeObject("Revolution" + RevolutionWrapProperties.Id.ToString()) + ":\n{\n";

			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.isSolid.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.isSolid.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.StartingAngle.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.StartingAngle.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.EndingAngle.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(RevolutionWrapProperties.EndingAngle.First().Value) + ",\n";

			foreach (var Dicts in RevolutionWrapProperties.PathLineDict)
			{
				dataJson += JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "},";
			}

			foreach (var Dicts in RevolutionWrapProperties.SketchPlane)
			{
				dataJson += "\n" + JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "},";
			}
			foreach (var Dicts in RevolutionWrapProperties.CurveArrArray)
			{
				dataJson += "\n" + JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "\n}";
			}

			dataJson = dataJson.Remove(dataJson.Length - 3, 1);
			dataJson += "\n},\n";
			return dataJson;
		}

		public static string ToJsonString(this BlendWrapParameters BlendWrapProperties)
		{
			string dataJson = "";
			dataJson += JsonConvert.SerializeObject("Blend" + BlendWrapProperties.Id.ToString()) + ":\n{\n";

			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.isSolid.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.isSolid.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.TopOffset.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.TopOffset.First().Value) + ",\n";

			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.BottomOffset.First().Key) + ":";
			dataJson += JsonConvert.SerializeObject(BlendWrapProperties.BottomOffset.First().Value) + ",\n";

			foreach (var Dicts in BlendWrapProperties.BaseSketchPlane)
			{
				dataJson += JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "},";
			}

			foreach (var Dicts in BlendWrapProperties.TopCurveArrArray)
			{
				dataJson += "\n" + JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "},";
			}

			foreach (var Dicts in BlendWrapProperties.BaseCurveArrArray)
			{
				dataJson += "\n" + JsonConvert.SerializeObject(Dicts.Key) + ":\n{\n";
				foreach (var dict in Dicts.Value)
				{
					foreach (var pair in dict)
					{
						dataJson += JsonConvert.SerializeObject(pair.Key) + ":";
						dataJson += JsonConvert.SerializeObject(pair.Value) + ",\n";
					}
				}
				dataJson = dataJson.Remove(dataJson.Length - 2, 1);
				dataJson += "\n}";
			}

			dataJson = dataJson.Remove(dataJson.Length - 3, 1);
			dataJson += "\n},\n";
			return dataJson;
		}
	}
}