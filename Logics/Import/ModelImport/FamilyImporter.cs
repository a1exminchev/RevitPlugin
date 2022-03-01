using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using ui = Autodesk.Revit.UI;
using CSh = CSharpFunctionalExtensions;
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
using Autodesk.Revit.DB.ExtensibleStorage;

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

			string schemaName = "OldDocData";
			Schema mySchema = Schema.ListSchemas().FirstOrDefault(x => x.SchemaName == schemaName);
			if (mySchema == null)
			{
				Guid guid = new Guid("681AB6EA-AE56-41A1-A880-B3918B417B0F");
				SchemaBuilder sb = new SchemaBuilder(guid);
				sb.SetWriteAccessLevel(AccessLevel.Public);
				sb.SetReadAccessLevel(AccessLevel.Public);
				sb.SetSchemaName(schemaName);
				sb.AddSimpleField("OldId", typeof(int)); //oldId - newId
				mySchema = sb.Finish();
			}

			//Creates constructor with new doc init
			_doc = doc;
		}


		public void Import(string JsonPath)
		{
			_json = File.ReadAllText(JsonPath);
			var famDoc = JsonConvert.DeserializeObject<FamilyDocumentData>(_json);
			_importers = CollectImporters();

			Transaction t1 = new Transaction(_doc, "Import");
			Transaction t2 = new Transaction(_doc, "Dimensions");
			using (t1)
            {
				t1.Start();
				foreach (var imp in _importers?.Values)
				{
					if (imp.Import(famDoc) != null)
                    {
						var dict = imp.Import(famDoc);
						foreach (AbstractTransfer i in dict.Values)
						{
							if (!i.GetType().Equals(typeof(DimensionTransfer)))
							{
								i.Create(_doc);
							}
						}
					}
				}
				t1.Commit();
			}
			using (t2)
            {
				t2.Start();
				foreach (var imp in _importers?.Values)
				{
					if (imp.Import(famDoc) != null)
					{
						var dict = imp.Import(famDoc);
						foreach (AbstractTransfer i in dict.Values)
						{
							if (i.GetType().Equals(typeof(DimensionTransfer)))
							{
								i.Create(_doc);
							}
						}
					}
				}
				t2.Commit();
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
	public static class ImportMethods
    {
		public static void SetIdEntityToElement(this Element el, int oldId)
		{
			string schemaName = "OldDocData";
			Schema mySchema = Schema.ListSchemas().FirstOrDefault(x => x.SchemaName == schemaName);
			Entity entity = new Entity(mySchema);
			Field OldId = mySchema.GetField("OldId");
			entity.Set<int>(OldId, oldId);
			el.SetEntity(entity);
		}
	}
}
