using System;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ArCm
{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class LayerTag : IExternalCommand
	{
		static AddInId addId = new AddInId(new Guid("DB33B475-E5B8-4517-B984-F52213965DB0"));
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var uiApp = commandData.Application;
			var app = commandData.Application.Application;
			var uidoc = uiApp.ActiveUIDocument;
			var doc = uidoc.Document;
			Category wall = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
			Category floor = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
			Category roof = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Roofs);
			CategorySet catSet = app.Create.NewCategorySet();
			catSet.Insert(wall);
			catSet.Insert(floor);
			catSet.Insert(roof);
			var floorIdFamilyCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().ToElementIds();
			Element floortypeEl = doc.GetElement(floorIdFamilyCollector.First());
			FloorType floortype = floortypeEl as FloorType;
			string parameterName = "00_Combining_Layers_Of_Construction";
			IList<Parameter> parList = floortype.GetParameters(parameterName);
			
			IList<int> nums = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			XYZ pickPoint1;
			Reference rf = null;
			try
            {
				rf = uidoc.Selection.PickObject(ObjectType.Element);
			}
			catch { }
			if (rf == null)
            {
				return Result.Cancelled;
            }
			Element el = doc.GetElement(rf.ElementId);
			Type elType = el.GetType();
			string selType = "";
			if (elType.ToString().Contains("Wall"))
			{
				selType = "Wall";
			}
			else if (elType.ToString().Contains("Roof"))
			{
				selType = "Roof";
			}
			else if (elType.ToString().Contains("Floor"))
			{
				selType = "Floor";
			}
			//Action//
			using (Transaction t = new Transaction(doc, "Layers tag"))
			{
				t.Start();
				if (parList.Count == 0)
				{
					Functions.ParameterOperations.CreateProjectParameterFromExistingSharedParameter(app, "00_Combining_Layers_Of_Construction", catSet, BuiltInParameterGroup.PG_DATA, false);
				}

				FilteredElementCollector cl = new FilteredElementCollector(doc).OfClass(typeof(Family));
				bool isETagLoaded = false;
				foreach (var fam in cl)
				{
					if (fam.Name == "Layers tag")
					{
						isETagLoaded = true;
					}
				}
				bool isRTagLoaded = false;
				foreach (var fam in cl)
				{
					if (fam.Name == "Выноска слоев")
					{
						isRTagLoaded = true;
					}
				}
				if (isETagLoaded == false)
				{
					doc.LoadFamily("Y:/TECHNICAL_LIBRARY/Revit/RVT_19/11_Architecture/01_Family_Library/Annotations/TRU_11_Layers_tag_eng.rfa");
				}
				if (isRTagLoaded == false)
				{
					doc.LoadFamily("Y:/TECHNICAL_LIBRARY/Revit/RVT_19/11_Architecture/01_Family_Library/Annotations/TRU_11_Layers_tag_rus.rfa");
				}
				bool isEng = false;
				bool isRight = false;
				if (selType == "Wall")
				{
					Wall w = el as Wall;
					WallType wtype = w.WallType;
					//Layers parameters//
					CompoundStructure str = wtype.GetCompoundStructure();
					IList<CompoundStructureLayer> layersList = str.GetLayers();
					string layText = "";
					IList<Parameter> layParList = wtype.GetParameters(parameterName);
					Parameter allLayPar = layParList[0];
					foreach (CompoundStructureLayer layer in layersList)
					{
						try
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
							string num = " (" + nums[0].ToString() + ")";
							//Setting params//
							layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
							if (Eng != null)
							{
								layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
								isEng = true;
							}
							nums.RemoveAt(0);
						}
						catch { }
						allLayPar.Set(layText);
					}
					BoundingBoxXYZ box = el.get_BoundingBox(doc.ActiveView);
					double Xmiddle = (box.Min.X + box.Max.X) / 2;
					double Zmiddle = (box.Min.Z + box.Max.Z) / 2;
					double Ymiddle = (box.Min.Y + box.Max.Y) / 2;
					XYZ point = new XYZ(Xmiddle, Ymiddle, Zmiddle);
					IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, true, TagMode.TM_ADDBY_MULTICATEGORY, TagOrientation.Horizontal, point);
					tag.LeaderEndCondition = LeaderEndCondition.Free;
					FamilySymbol tagSym = doc.GetElement(tag.GetTypeId()) as FamilySymbol;
					SketchPlane sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin));
					doc.ActiveView.SketchPlane = sp;
					try
                    {
						pickPoint1 = uidoc.Selection.PickPoint();
					}
                    catch
					{
						return Result.Cancelled;
					}

					TaskDialog mainDialog = new TaskDialog("User menu");
					mainDialog.MainInstruction = "Pick the side of tag";
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Tag for left");
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Tag for right");
					mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
					mainDialog.DefaultButton = TaskDialogResult.Close;
					TaskDialogResult tResult = mainDialog.Show();
					if (TaskDialogResult.CommandLink1 == tResult)
					{
						isRight = false;
					}
					else if (TaskDialogResult.CommandLink2 == tResult)
					{
						isRight = true;
					}
					if (isEng == true && isRight == true)
					{
						string typeName = "Right (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == true)
					{
						string typeName = "Правый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == true && isRight == false)
					{
						string typeName = "Left (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == false)
					{
						string typeName = "Левый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					tag.TagHeadPosition = pickPoint1;
					tag.LeaderEnd = new XYZ(point.X, point.Y, pickPoint1.Z);
					doc.Delete(sp.Id);
				}
				else if (selType == "Roof")
				{
					RoofBase r = el as RoofBase;
					RoofType rtype = r.RoofType;
					//Layers parameters//
					CompoundStructure str = rtype.GetCompoundStructure();
					IList<CompoundStructureLayer> layersList = str.GetLayers();
					string layText = "";
					IList<Parameter> layParList = rtype.GetParameters(parameterName);
					Parameter allLayPar = layParList[0];
					foreach (CompoundStructureLayer layer in layersList)
					{
						try
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
							string num = " (" + nums[0].ToString() + ")";
							//Setting params//
							layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
							if (Eng != null)
							{
								layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
								isEng = true;
							}
							nums.RemoveAt(0);
						}
						catch { }
						allLayPar.Set(layText);
					}
					BoundingBoxXYZ box = el.get_BoundingBox(doc.ActiveView);
					double Xmiddle = (box.Min.X + box.Max.X) / 2;
					double Zmiddle = (box.Min.Z + box.Max.Z) / 2;
					double Ymiddle = (box.Min.Y + box.Max.Y) / 2;
					XYZ point = new XYZ(Xmiddle, Ymiddle, Zmiddle);
					IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, true, TagMode.TM_ADDBY_MULTICATEGORY, TagOrientation.Horizontal, point);
					tag.LeaderEndCondition = LeaderEndCondition.Free;
					FamilySymbol tagSym = doc.GetElement(tag.GetTypeId()) as FamilySymbol;
					SketchPlane sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin));
					doc.ActiveView.SketchPlane = sp;
					pickPoint1 = uidoc.Selection.PickPoint();
					TaskDialog mainDialog = new TaskDialog("User menu");
					mainDialog.MainInstruction = "Pick the side of tag";
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Tag for left");
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Tag for right");
					mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
					mainDialog.DefaultButton = TaskDialogResult.Close;
					TaskDialogResult tResult = mainDialog.Show();
					if (TaskDialogResult.CommandLink1 == tResult)
					{
						isRight = false;
					}
					else if (TaskDialogResult.CommandLink2 == tResult)
					{
						isRight = true;
					}
					if (isEng == true && isRight == true)
					{
						string typeName = "Right (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == true)
					{
						string typeName = "Правый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == true && isRight == false)
					{
						string typeName = "Left (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == false)
					{
						string typeName = "Левый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					tag.TagHeadPosition = pickPoint1;
					if (pickPoint1.Z > Zmiddle)
					{
						tag.LeaderEnd = new XYZ(pickPoint1.X, point.Y, box.Max.Z);
					}
					else
					{
						tag.LeaderEnd = new XYZ(pickPoint1.X, point.Y, box.Min.Z);
					}
					tag.LeaderElbow = pickPoint1;
					doc.Delete(sp.Id);
				}
				else if (selType == "Floor")
				{
					Floor f = el as Floor;
					FloorType ftype = f.FloorType;
					//Layers parameters//
					CompoundStructure str = ftype.GetCompoundStructure();
					IList<CompoundStructureLayer> layersList = str.GetLayers();
					string layText = "";
					IList<Parameter> layParList = ftype.GetParameters(parameterName);
					Parameter allLayPar = layParList[0];
					foreach (CompoundStructureLayer layer in layersList)
					{
						try
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
							string num = " (" + nums[0].ToString() + ")";
							//Setting params//
							layText = layText.Insert(layText.Length, " " + Rus + " " + WidR + num + "\n");
							if (Eng != null)
							{
								layText = layText.Insert(layText.Length, " " + Eng + " " + WidE + num + "\n");
								isEng = true;
							}
							nums.RemoveAt(0);
						}
						catch { }
						allLayPar.Set(layText);
					}
					BoundingBoxXYZ box = el.get_BoundingBox(doc.ActiveView);
					double Xmiddle = (box.Min.X + box.Max.X) / 2;
					double Zmiddle = (box.Min.Z + box.Max.Z) / 2;
					double Ymiddle = (box.Min.Y + box.Max.Y) / 2;
					XYZ point = new XYZ(Xmiddle, Ymiddle, Zmiddle);
					IndependentTag tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, true, TagMode.TM_ADDBY_MULTICATEGORY, TagOrientation.Horizontal, point);
					tag.LeaderEndCondition = LeaderEndCondition.Free;
					FamilySymbol tagSym = doc.GetElement(tag.GetTypeId()) as FamilySymbol;
					SketchPlane sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin));
					doc.ActiveView.SketchPlane = sp;
					pickPoint1 = uidoc.Selection.PickPoint();
					TaskDialog mainDialog = new TaskDialog("User menu");
					mainDialog.MainInstruction = "Pick the side of tag";
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Tag for left");
					mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Tag for right");
					mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
					mainDialog.DefaultButton = TaskDialogResult.Close;
					TaskDialogResult tResult = mainDialog.Show();
					if (TaskDialogResult.CommandLink1 == tResult)
					{
						isRight = false;
					}
					else if (TaskDialogResult.CommandLink2 == tResult)
					{
						isRight = true;
					}
					if (isEng == true && isRight == true)
					{
						string typeName = "Right (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == true)
					{
						string typeName = "Правый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == true && isRight == false)
					{
						string typeName = "Left (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					else if (isEng == false && isRight == false)
					{
						string typeName = "Левый (" + layersList.Count.ToString() + ")";
						Element tagType = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == typeName).SingleOrDefault();
						tag.GetParameters("Type")[0].Set(tagType.Id);
					}
					tag.TagHeadPosition = pickPoint1;
					if (pickPoint1.Z > Zmiddle)
					{
						tag.LeaderEnd = new XYZ(pickPoint1.X, point.Y, box.Max.Z);
					}
					else
					{
						tag.LeaderEnd = new XYZ(pickPoint1.X, point.Y, box.Min.Z);
					}
					tag.LeaderElbow = pickPoint1;
					doc.Delete(sp.Id);
				}
				IList<Element> tagTypesR = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name.Contains("Right (")).ToList();
				IList<Element> tagTypesL = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name.Contains("Left (")).ToList();
				IList<Element> tagTypesLR = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name.Contains("Левый (")).ToList();
				IList<Element> tagTypesRR = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name.Contains("Правый (")).ToList();
				foreach (Element elR in tagTypesR)
				{
					FamilySymbol tagSym = elR as FamilySymbol;
					IList<Parameter> listSymPar = tagSym.GetParameters("Leader Arrowhead");
					Parameter symLeader = listSymPar[0];
					Element arrow = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).Where(x => x.Name == "TRU_Arrow_3_15°_Filled").SingleOrDefault();
					symLeader.Set(arrow.Id);
				}
				foreach (Element elL in tagTypesL)
				{
					FamilySymbol tagSym = elL as FamilySymbol;
					IList<Parameter> listSymPar = tagSym.GetParameters("Leader Arrowhead");
					Parameter symLeader = listSymPar[0];
					Element arrow = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).Where(x => x.Name == "TRU_Arrow_3_15°_Filled").SingleOrDefault();
					symLeader.Set(arrow.Id);
				}
				foreach (Element elLR in tagTypesLR)
				{
					FamilySymbol tagSym = elLR as FamilySymbol;
					IList<Parameter> listSymPar = tagSym.GetParameters("Leader Arrowhead");
					Parameter symLeader = listSymPar[0];
					Element arrow = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).Where(x => x.Name == "TRU_Arrow_3_15°_Filled").SingleOrDefault();
					symLeader.Set(arrow.Id);
				}
				foreach (Element elRR in tagTypesRR)
				{
					FamilySymbol tagSym = elRR as FamilySymbol;
					IList<Parameter> listSymPar = tagSym.GetParameters("Leader Arrowhead");
					Parameter symLeader = listSymPar[0];
					Element arrow = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).Where(x => x.Name == "TRU_Arrow_3_15°_Filled").SingleOrDefault();
					symLeader.Set(arrow.Id);
				}
				t.Commit();
			}
			return Result.Succeeded;
		}
	}
}