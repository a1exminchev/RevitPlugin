using System;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace ArCm
{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class RegisterLayerUpdater : IExternalCommand
	{
		static AddInId addId = new AddInId(new Guid("DB33B475-E5B8-4517-B984-F52213967DB0"));
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var result = Result.Succeeded;
			var uiApp = commandData.Application;
			var app = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			LayerUpdate updater = new LayerUpdate(app.ActiveAddInId);
			UpdaterRegistry.RegisterUpdater(updater);
			ElementClassFilter fTypeFilter = new ElementClassFilter(typeof(FloorType));
			ElementClassFilter wTypeFilter = new ElementClassFilter(typeof(WallType));
			ElementClassFilter rTypeFilter = new ElementClassFilter(typeof(RoofType));
			UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), fTypeFilter, Element.GetChangeTypeAny());
			UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wTypeFilter, Element.GetChangeTypeAny());
			UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), rTypeFilter, Element.GetChangeTypeAny());
			return result;
		}

		public class LayerUpdate : IUpdater
		{
			static AddInId m_appId;
			static UpdaterId m_updaterId;
			// constructor takes the AddInId for the add-in associated with this updater
			public LayerUpdate(AddInId id)
			{
				m_appId = id;
				// every Updater must have a unique ID
				m_updaterId = new UpdaterId(m_appId, new Guid("FBABF8B2-4C06-42d4-97C1-D1B5EB593EFF"));
			}
			public void Execute(UpdaterData data)
			{
				Document doc = data.GetDocument();
				var wallFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToElementIds();
				var floorFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().ToElementIds();
				var roofFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsElementType().ToElementIds();
				string parameterName = "00_Combining_Layers_Of_Construction";
				foreach (ElementId wtypeId in wallFamilyCollector)
				{
					WallType wt = doc.GetElement(wtypeId) as WallType;
					if (data.IsChangeTriggered(wt.Id, Element.GetChangeTypeAny()))
					{
						IList<int> numsW = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
						try
						{
							if (wt.Kind.ToString() != "Curtain" && wt.Kind.ToString() != "Stacked")
							{
								CompoundStructure str = wt.GetCompoundStructure();
								IList<CompoundStructureLayer> layersList = str.GetLayers();
								string layText = "";
								IList<Parameter> layParList = wt.GetParameters(parameterName);
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
									if (Eng != null)
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
				}
				foreach (ElementId ftId in floorFamilyCollector)
				{
					FloorType ft = doc.GetElement(ftId) as FloorType;
					if (data.IsChangeTriggered(ft.Id, Element.GetChangeTypeAny()))
					{
						try
						{
							IList<int> numsF = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
							CompoundStructure str = ft.GetCompoundStructure();
							IList<CompoundStructureLayer> layersList = str.GetLayers();
							string layText = "";
							IList<Parameter> layParList = ft.GetParameters(parameterName);
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
				}
				foreach (ElementId rtId in roofFamilyCollector)
				{
					RoofType rt = doc.GetElement(rtId) as RoofType;
					if (data.IsChangeTriggered(rt.Id, Element.GetChangeTypeAny()))
					{
						try
						{
							IList<int> numsR = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
							CompoundStructure str = rt.GetCompoundStructure();
							IList<CompoundStructureLayer> layersList = str.GetLayers();
							string layText = "";
							IList<Parameter> layParList = rt.GetParameters(parameterName);
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
				}
			}
			public string GetAdditionalInformation() { return "Layer updater"; }
			public ChangePriority GetChangePriority() { return ChangePriority.FloorsRoofsStructuralWalls; }
			public UpdaterId GetUpdaterId() { return m_updaterId; }
			public string GetUpdaterName() { return "Layer updater"; }
		}
	}
}