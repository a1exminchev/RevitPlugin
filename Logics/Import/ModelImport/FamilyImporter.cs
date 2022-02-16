using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using ui = Autodesk.Revit.UI;
using CSharpFunctionalExtensions;
using Logics.Export.ModelExport.Extractors;
using Logics.Export.ModelExport.Extractors.Implementations;
using Logics.Export.Wraps.Implementations;
using Logics.Export.Wraps.Interfaces;
using PluginUtil;
using PluginUtil.Loger;
using Newtonsoft.Json;
using Logics.Export;
using Logics.Import.ModelImport.Importers;
using Logics.Import.Transforms;

namespace Logics.Import.ModelImport
{
    public class FamilyImporter
    {
		private readonly Document _doc;
		private string _json;
		private Dictionary<Type, IImporter> _importers;
		public FamilyImporter(Document doc)
		{
			if (!doc.IsFamilyDocument)
			{
				throw new Exception($"Document {doc.Title} is not family document");
			}
			//Creates constructor with new doc init
			_doc = doc;
		}


		public void Import(string JsonPath)
		{
			_json = File.ReadAllText(JsonPath);
			var famDoc = JsonConvert.DeserializeObject<FamilyDocumentData>(_json);
			_importers = CollectImporters();

			Transaction t = new Transaction(_doc, "Import");
			using (t)
            {
				t.Start();
				foreach (var imp in _importers.Values)
				{
					var dict = imp.Import(famDoc);
					foreach (AbstractTransfer i in dict.Values)
					{
						i.Create(_doc);
					}
				}
				t.Commit();
			}
		}

		private Dictionary<Type, IImporter> CollectImporters()
		{
			var imptrs = new Dictionary<Type, IImporter>();
			var importers = Assembly.GetAssembly(typeof(IImporter))
									 .GetTypes()
									 .Where(x => x.HaveInterface(typeof(IImporter)))
									 .Where(x => !x.IsAbstract)
									 .ToList();

			foreach (var type in importers)
			{
				var genType = type.BaseType.GetGenericArguments().FirstOrDefault();
				var imptr = CreateInstance(type);
				imptrs[genType] = imptr;
			}
			
			return imptrs;
		}

		private IImporter CreateInstance(Type type)
		{
			type.GetConstructors().Select(x => x.GetParameters().Length).Debug();
			var constr = type.GetConstructors()
							 .Where(x => x.GetParameters().Length == 1)
							 .FirstOrDefault(x => x.GetParameters().FirstOrDefault().ParameterType == typeof(string));

			return (IImporter)constr.Invoke(new object[] { _json });
		}
	}
}
