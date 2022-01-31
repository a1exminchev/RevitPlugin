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
			Configure.ConfigureLogger( );

			var familyExporter = new FamilyExporter(uidoc.Document);
			try
			{
				var FamilyWrap = familyExporter.Export();
				string dataJson = "{\n" + JsonConvert.SerializeObject("Extrusions") + ":\n{\n";
				foreach (var pair in FamilyWrap.Extrusions)
				{
					dataJson += pair.Value.ExtrusionWrapProperties.ToJsonString();
				}
				if (FamilyWrap.Extrusions.Count != 0)
                {
					dataJson = dataJson.Remove(dataJson.Length - 2, 1) + "},\n";
				}
				else
                {
					dataJson += "\n},\n";
				}
				dataJson += JsonConvert.SerializeObject("Revolutions") + ":\n{\n";
				foreach (var pair in FamilyWrap.Revolutions)
				{
					dataJson += pair.Value.RevolutionWrapProperties.ToJsonString();
				}
				if (FamilyWrap.Revolutions.Count != 0)
                {
					dataJson = dataJson.Remove(dataJson.Length - 2, 1) + "}";
				}
				else
                {
					dataJson += "\n}";
				}
				dataJson += "\n}";
				TextWriter tw = new StreamWriter(GlobalData.PluginDir + @"\StudyTask\Files\FamilyData.json");
				using (tw)
                {
					tw.Write(dataJson);
				}
			}
			catch (Exception e)
			{
				e.LogError();
			}


			return Result.Succeeded;
		}
	}
}