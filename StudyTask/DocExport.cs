using System;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.FamilyExport.ModelExport;
using PluginUtil.Loger;
using SCOPE_RevitPluginLogic.Utils;

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
			Configure.ConfigureLogger( );
			var familyExporter = new FamilyExporter(uidoc.Document);
			try {
				var FamilyWrap = familyExporter.Export();
				string data = (FamilyWrap.Extrusions.First().Value.LengthOfExtrusion * 304,8) + " / " +
							  FamilyWrap.Extrusions.First().Value.AmountOfSketchLines.ToString() + " lines";
				Log.WriteLog(data, Log.LogPath);
			} catch (Exception e) {
				e.LogError();
			}


			return Result.Succeeded;
		}
	}
}