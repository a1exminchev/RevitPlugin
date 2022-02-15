﻿using System;
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
			crDocument crDoc = doc.Create;
			Configure.ConfigureLogger();
			//Transaction t = new Transaction(doc, "test");
			//using (t)
			//{
			//	t.Start();

			//	//var columns = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsElementType().ToElements();
			//	//var levels = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements();
			//	//Level lvl = levels.FirstOrDefault() as Level;
			//	//var pick = uidoc.Selection.PickObject(ObjectType.Element);
			//	//var famIns = doc.GetElement(pick) as FamilyInstance;
			//	//var famSym = famIns.Symbol;
			//	//var pt = uidoc.Selection.PickPoint();
			//	//crDoc.NewFamilyInstance(pt, famSym, lvl, Autodesk.Revit.DB.Structure.StructuralType.Column);
			//	//TaskDialog.Show("Type", lvl.Name);

			//	t.Commit();
			//}


			return Result.Succeeded;
		}
	}
}