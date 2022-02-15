using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using imp = Logics.Geometry.Implementation;
using Logics.Export;

namespace Logics.Import.Transforms
{
    public class ExtrusionTransfer : AbstractTransfer
    {
		public ExtrusionTransfer() {
			
        }
		//Отвечает за создание формы в модели по полученному экземпляру из Json
		public ExtrusionWrapParameters ExtrusionWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			imp.ExtrusionParameters exParams = new imp.ExtrusionParameters();

			exParams.isSolid = ExtrusionWrapProperties.IsSolid;

			exParams.EndOffset = ExtrusionWrapProperties.EndOffset;

			exParams.StartOffset = ExtrusionWrapProperties.StartOffset;

			XYZ skOrigin = new XYZ(ExtrusionWrapProperties.SketchPlane[0]["SketchOrigin"][0],
										  ExtrusionWrapProperties.SketchPlane[0]["SketchOrigin"][1],
										  ExtrusionWrapProperties.SketchPlane[0]["SketchOrigin"][2]);
			XYZ skNormal = new XYZ(ExtrusionWrapProperties.SketchPlane[1]["SketchNormal"][0],
										  ExtrusionWrapProperties.SketchPlane[1]["SketchNormal"][1],
										  ExtrusionWrapProperties.SketchPlane[1]["SketchNormal"][2]);
			exParams.SketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(skNormal, skOrigin));


			CurveArrArray curArrArr = new CurveArrArray();
			CurveArray curveArray = new CurveArray();
			int numArr = 1;
			int numLine = 1;
			foreach (var dict in ExtrusionWrapProperties.CurveArrArray)
            {
				if (dict.FirstOrDefault().Key.Split('y')[1].Split('L')[0] == numArr.ToString())
                {
					if (dict.FirstOrDefault().Key.Split('e')[1] == numLine.ToString())
                    {
						curveArray.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
																   dict.FirstOrDefault().Value[1],
																   dict.FirstOrDefault().Value[2]),
														   new XYZ(dict.FirstOrDefault().Value[3],
																   dict.FirstOrDefault().Value[4],
																   dict.FirstOrDefault().Value[5])));
						numLine += 1;
					}
				}
				else if (dict.FirstOrDefault().Key.Split('y')[1].Split('A')[0] == numArr.ToString())
                {
					if (dict.FirstOrDefault().Key.Split('c')[1] == numLine.ToString())
					{
						curveArray.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
															 dict.FirstOrDefault().Value[1],
															 dict.FirstOrDefault().Value[2]),
													 new XYZ(dict.FirstOrDefault().Value[3],
															 dict.FirstOrDefault().Value[4],
															 dict.FirstOrDefault().Value[5]),
													 new XYZ(dict.FirstOrDefault().Value[6],
															 dict.FirstOrDefault().Value[7],
															 dict.FirstOrDefault().Value[8])));

						numLine += 1;
					}
				}
                else
				{
					numArr += 1;
					numLine = 1;
					curArrArr.Append(curveArray);
					curveArray = new CurveArray();
					if (dict.FirstOrDefault().Key.Contains("Line"))
                    {
						curveArray.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
																   dict.FirstOrDefault().Value[1],
																   dict.FirstOrDefault().Value[2]),
														   new XYZ(dict.FirstOrDefault().Value[3],
																   dict.FirstOrDefault().Value[4],
																   dict.FirstOrDefault().Value[5])));
					}
					else if (dict.FirstOrDefault().Key.Contains("Arc"))
                    {
						curveArray.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
															 dict.FirstOrDefault().Value[1],
															 dict.FirstOrDefault().Value[2]),
													 new XYZ(dict.FirstOrDefault().Value[3],
															 dict.FirstOrDefault().Value[4],
															 dict.FirstOrDefault().Value[5]),
													 new XYZ(dict.FirstOrDefault().Value[6],
															 dict.FirstOrDefault().Value[7],
															 dict.FirstOrDefault().Value[8])));
					}
					numLine = 2;
				}
			}
			curArrArr.Append(curveArray);
			exParams.curveArrArray = curArrArr;

			imp.ExtrusionCreator extrusionCreator = new imp.ExtrusionCreator(docToImport, exParams);
			extrusionCreator.Create();
		}
	}
}
