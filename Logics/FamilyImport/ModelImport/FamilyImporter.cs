using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using ui = Autodesk.Revit.UI;
using CSharpFunctionalExtensions;
using Logics.FamilyExport.ModelExport.Extractors;
using Logics.FamilyExport.ModelExport.Extractors.Implementations;
using Logics.FamilyExport.Wraps.Implementations;
using Logics.FamilyExport.Wraps.Interfaces;
using PluginUtil;
using PluginUtil.Loger;
using Newtonsoft.Json;
using Logics.FamilyExport;
using Logics.FamilyImport.ModelImport.Importers;
using Logics.FamilyImport.Transforms;

namespace Logics.FamilyImport.ModelImport
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
			
			_importers = CollectImporters();

			foreach (var imp in _importers.Values)
            {
				var dict = imp.Import();
				foreach (AbstractTransfer i in dict.Values)
                {
					i.Create(_doc);
                }
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
