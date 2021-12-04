using System;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Functions;

namespace ArCm
{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class LayerTagRefresh : IExternalCommand
	{
		static AddInId addId = new AddInId(new Guid("DB33B475-E5B8-4517-B984-F52213967DB0"));
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var result = Result.Succeeded;
			var uiApp = commandData.Application;
			var app = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			var doc = uidoc.Document;
			var floorIdFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().ToElementIds();
			Category wall = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
			Category floor = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
			Category roof = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Roofs);
			CategorySet catSet = app.Create.NewCategorySet();
			catSet.Insert(wall);
			catSet.Insert(floor);
			catSet.Insert(roof);
			var wallFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToElementIds();
			var roofFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsElementType().ToElementIds();
			var floorFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().ToElementIds();
			Element floortypeEl = doc.GetElement(floorIdFamilyCollector.First());
			FloorType floortype = floortypeEl as FloorType;
			string parameterName = "00_Combining_Layers_Of_Construction";
			IList<Parameter> parList = floortype.GetParameters(parameterName);
			using (Transaction t = new Transaction(doc, "Layers tag"))
			{
				t.Start();
				if (parList.Count == 0)
				{
					GlobalSharedProjectParameters.CreateProjectParameterFromExistingSharedParameter(app, parameterName, catSet, BuiltInParameterGroup.PG_DATA, false);
				}
				foreach (ElementId wtypeId in wallFamilyCollector)
				{
					Element wtypeEl = doc.GetElement(wtypeId);
					WallType wtype = wtypeEl as WallType;
					IList<int> numsW = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
					try
					{
						if (wtype.Kind.ToString() != "Curtain" && wtype.Kind.ToString() != "Stacked")
						{
							CompoundStructure str = wtype.GetCompoundStructure();
							IList<CompoundStructureLayer> layersList = str.GetLayers();
							string layText = "";
							IList<Parameter> layParList = wtype.GetParameters(parameterName);
							Parameter allLayPar = layParList[0];
							foreach (CompoundStructureLayer layer in layersList)
							{
								double layerWidth = layer.Width * 304.8;
								string WidR = layerWidth.ToString() + "мм";
								string WidE = layerWidth.ToString() + "mm";
								ElementId matId = layer.MaterialId;
								Material mat = doc.GetElement(matId) as Material;
								IList<Parameter> listRus = mat.GetParameters("00_Descriptions_Material_RUS");
								IList<Parameter> listEng = mat.GetParameters("00_Descriptions_Material_ENG");
								string Rus = "";
								string Eng = "";
								Rus = listRus[0].AsString();
								Eng = listEng[0].AsString();
								string num = " (" + numsW[0].ToString() + ")";
								//Setting params//
								layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
								if (Eng != "")
								{
									layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
								}
								numsW.RemoveAt(0);
							}
							allLayPar.Set(layText);
						}
					}
					catch { }
				}

				foreach (ElementId ftypeId in floorFamilyCollector)
				{
					try
					{
						IList<int> numsF = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
						Element ftypeEl = doc.GetElement(ftypeId);
						FloorType ftype = ftypeEl as FloorType;
						CompoundStructure str = ftype.GetCompoundStructure();
						IList<CompoundStructureLayer> layersList = str.GetLayers();
						string layText = "";
						IList<Parameter> layParList = ftype.GetParameters(parameterName);
						Parameter allLayPar = layParList[0];
						foreach (CompoundStructureLayer layer in layersList)
						{
							double layerWidth = layer.Width * 304.8;
							string WidR = layerWidth.ToString() + "мм";
							string WidE = layerWidth.ToString() + "mm";
							ElementId matId = layer.MaterialId;
							Material mat = doc.GetElement(matId) as Material;
							IList<Parameter> listRus = mat.GetParameters("00_Descriptions_Material_RUS");
							IList<Parameter> listEng = mat.GetParameters("00_Descriptions_Material_ENG");
							string Rus = "";
							string Eng = "";
							Rus = listRus[0].AsString();
							Eng = listEng[0].AsString();
							string num = " (" + numsF[0].ToString() + ")";
							//Setting params//
							layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
							if (Eng != null)
							{
								layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
							}
							numsF.RemoveAt(0);
						}
						allLayPar.Set(layText);
					}
					catch { }
				}

				foreach (ElementId rtypeId in roofFamilyCollector)
				{
					try
					{
						IList<int> numsR = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
						Element rtypeEl = doc.GetElement(rtypeId);
						RoofType rtype = rtypeEl as RoofType;
						CompoundStructure str = rtype.GetCompoundStructure();
						IList<CompoundStructureLayer> layersList = str.GetLayers();
						string layText = "";
						IList<Parameter> layParList = rtype.GetParameters(parameterName);
						Parameter allLayPar = layParList[0];
						foreach (CompoundStructureLayer layer in layersList)
						{
							double layerWidth = layer.Width * 304.8;
							string WidR = layerWidth.ToString() + "мм";
							string WidE = layerWidth.ToString() + "mm";
							ElementId matId = layer.MaterialId;
							Material mat = doc.GetElement(matId) as Material;
							IList<Parameter> listRus = mat.GetParameters("00_Descriptions_Material_RUS");
							IList<Parameter> listEng = mat.GetParameters("00_Descriptions_Material_ENG");
							string Rus = "";
							string Eng = "";
							Rus = listRus[0].AsString();
							Eng = listEng[0].AsString();
							string num = " (" + numsR[0].ToString() + ")";
							//Setting params//
							layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
							if (Eng != null)
							{
								layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
							}
							numsR.RemoveAt(0);
						}
						allLayPar.Set(layText);
					}
					catch { }
				}
				t.Commit();
			}
			TaskDialog.Show("Message", "Successfully updated");
			return result;
		}
	}
}