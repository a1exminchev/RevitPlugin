using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using imp = Logics.Geometry.Implementation;
using Logics.Export;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Logics.Import.Transforms
{
    public class DimensionTransfer : AbstractTransfer
    {
		public DimensionTransfer() {
			
        }
		//Отвечает за создание размера в модели по полученному экземпляру из Json
		public DimensionWrapParameters DimensionWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			string viewName = DimensionWrapProperties.ViewName;
			var clView = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType();
			View view = clView.Where(x => x.Name == viewName).FirstOrDefault() as View;

			var dict = DimensionWrapProperties.LineAlongDim;
			Line line = Line.CreateUnbound(new XYZ(dict.FirstOrDefault().Value[0],
												 dict.FirstOrDefault().Value[1],
												 dict.FirstOrDefault().Value[2]),
										 new XYZ(dict.FirstOrDefault().Value[3],
												 dict.FirstOrDefault().Value[4],
												 dict.FirstOrDefault().Value[5]));

			ReferenceArray refArr = new ReferenceArray();
			var clEl = new FilteredElementCollector(docToImport).WhereElementIsNotElementType().ToElements().Where(x => x.Id.IntegerValue > 2200).ToList();
			string schemaName = "OldDocData";
			Schema mySchema = Schema.ListSchemas().FirstOrDefault(x => x.SchemaName == schemaName);
			Options opt = new Options();
			opt.ComputeReferences = true;
			opt.DetailLevel = ViewDetailLevel.Fine;
			opt.IncludeNonVisibleObjects = true;
			foreach (int[] arr in DimensionWrapProperties.ReferenceIds)
            {
				Element el = null;
				el = clEl.Where(x => x.GetEntity(mySchema).IsValid()).FirstOrDefault(x => x?.GetEntity(mySchema)?.Get<int>("OldId")  == arr[0]);
				if (arr.Length == 1)
                {
					ReferencePlane refPlane = el as ReferencePlane;
					if (refPlane != null)
                    {
						Reference refRefPlane = refPlane.GetReference();
						refArr.Append(refRefPlane);
                    }
					ModelCurve modelCurve = el as ModelCurve;
					if (modelCurve != null)
                    {
						Reference refModelLine = modelCurve?.GeometryCurve.Reference;
						refArr.Append(refModelLine);
					}
				}
				else
                {
					GeometryElement geoEl = el?.get_Geometry(opt);
					if (geoEl?.Count() != 0 && geoEl != null)
                    {
						foreach (GeometryObject geoObj in geoEl)
                        {
							Solid solid = geoObj as Solid;
							if (solid != null && solid.Faces.Size > 0)
                            {
								foreach (Face face in solid.Faces)
                                {
									if (arr[1] == face.Id)
                                    {
										Reference refFace = face.Reference;
										refArr.Append(refFace);
                                    }
                                }
							}
							if (solid != null && solid.Edges.Size > 0)
							{
								foreach (Edge edge in solid.Edges)
								{
									if (arr[1] == edge.Id)
									{
										Reference refEdge = edge.Reference;
										refArr.Append(refEdge);
									}
								}
							}
						}
					}
				}
			}
			
			Dimension dim = docToImport.FamilyCreate.NewDimension(view, line, refArr);
		}
	}
}
