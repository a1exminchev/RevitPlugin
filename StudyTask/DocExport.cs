using System;
using System.IO;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.Export.ModelExport;
using Logics.Export;
using PluginUtil.Loger;
using SCOPE_RevitPluginLogic.Utils;
using Newtonsoft.Json;
using PluginUtils;

namespace StudyTask{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class DocExport : IExternalCommand{
		public Result Execute(ExternalCommandData commandData
		                      , ref string        message
		                      , ElementSet        elements) {
			var uiApp = commandData.Application;
			var app   = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			var doc = uidoc.Document;
			Configure.ConfigureLogger( );
			if (doc.IsFamilyDocument == true)
			{
				var familyExporter = new FamilyExporter(doc);
				var familyWrap = familyExporter.GetFamDocWrap();
				familyExporter.ExportToJson(GlobalData.PluginDir + @"\StudyTask\Files\FamilyData.json", familyWrap);
			}
			else
            {
				var projExporter = new ProjectExporter(doc);
				var projWrap = projExporter.GetProjDocWrap();
				projExporter.ExportToJson(GlobalData.PluginDir + @"\StudyTask\Files\ProjectData.json", projWrap);
			}

			return Result.Succeeded;
		}
	}
}