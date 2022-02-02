using System;
using System.IO;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.FamilyExport.ModelExport;
using Logics.FamilyExport;
using PluginUtil.Loger;
using SCOPE_RevitPluginLogic.Utils;
using PluginLogics;
using Newtonsoft.Json;

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

			var familyExporter = new FamilyExporter(doc);
			var FamilyWrap = familyExporter.GetFamDocWrap();
			try
			{
				familyExporter.ExportToJson(GlobalData.PluginDir + @"\StudyTask\Files\FamilyData.json", FamilyWrap);
			}
			catch (Exception e)
			{
				e.LogError();
			}


			return Result.Succeeded;
		}
	}
}