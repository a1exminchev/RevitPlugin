using System;
using System.IO;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.FamilyImport;
using Logics.FamilyImport.ModelImport;
using PluginUtil.Loger;
using SCOPE_RevitPluginLogic.Utils;
using Newtonsoft.Json;
using Logics.FamilyImport.Transforms;
using PluginUtils;

namespace StudyTask{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class DocImport : IExternalCommand{
		public Result Execute(ExternalCommandData commandData
		                      , ref string        message
		                      , ElementSet        elements) {
			var uiApp = commandData.Application;
			var app   = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			Configure.ConfigureLogger( );

			FamilyCreator familyCreator = new FamilyCreator(app);
			Document newDoc = familyCreator.CreateNewFamily(uiApp, "FamilyWithImport", "Metric Generic Model");

			FamilyImporter familyImporter = new FamilyImporter(newDoc);

			Transaction t = new Transaction(newDoc, "import");
			using (t)
			{
				t.Start();

				familyImporter.Import(GlobalData.PluginDir + @"\StudyTask\Files\FamilyData.json");

				t.Commit();
			}


			return Result.Succeeded;
		}
	}
}