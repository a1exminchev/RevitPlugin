using System;
using System.IO;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using crDocument = Autodesk.Revit.Creation.Document;
using Logics.Import;
using Logics.Import.ModelImport;
using PluginUtil.Loger;
using SCOPE_RevitPluginLogic.Utils;
using Newtonsoft.Json;
using Logics.Import.Transforms;
using PluginUtils;
using Autodesk.Revit.UI.Selection;
using Logics.Export.ModelExport;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace StudyTask{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class TestVoid : IExternalCommand{
		public Result Execute(ExternalCommandData commandData
		                      , ref string        message
		                      , ElementSet        elements) {
			var uiApp = commandData.Application;
			var app   = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			var doc = uidoc.Document;
			//crDocument crDoc = doc.Create;
			Configure.ConfigureLogger();
            Transaction t = new Transaction(doc, "test");
            using (t)
            {
                t.Start();

				//            var pick = uidoc.Selection.PickObject(ObjectType.Element);
				//var el = doc.GetElement(pick) as FamilyInstance;
				//string text = "";
				//foreach (Parameter par in el.Parameters)
				//            {
				//	try
				//                {
				//		text += JsonConvert.SerializeObject(par);
				//	}
				//                catch { }
				//            }
				//TextWriter textWriter = File.CreateText(GlobalData.PluginDir + @"\StudyTask\Files\FamilyData.json");
				//textWriter.Write(text);

				var pick = uidoc.Selection.PickObject(ObjectType.Element);
				var el = doc.GetElement(pick);
				string schemaName = "OldDocData";
				Schema mySchema = Schema.ListSchemas().FirstOrDefault(x => x.SchemaName == schemaName);
                try
                {
					TaskDialog.Show("Ids", el.Id + " - " + el?.GetEntity(mySchema)?.Get<int>("OldId"));
				}
                catch { }

				t.Commit();
            }


            return Result.Succeeded;
		}
	}
}