using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Logics.Export;
using Logics.Import.ModelImport;
using imp = Logics.Geometry.Implementation;

namespace Logics.Import.Transforms
{
    public class RevolutionTransfer : AbstractTransfer
    {
		public RevolutionTransfer() {
			
        }
		//Отвечает за создание формы в модели по полученному экземпляру из Json
		public RevolutionWrapParameters RevolutionWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			imp.RevolutionParameters revParams = new imp.RevolutionParameters();

			revParams.isSolid = RevolutionWrapProperties.IsSolid;

			revParams.EndingAngle = RevolutionWrapProperties.EndingAngle;

			revParams.StartingAngle = RevolutionWrapProperties.StartingAngle;

			Line axisLine = Line.CreateBound(new XYZ(RevolutionWrapProperties.PathLineDict["PathLine"][0],
													RevolutionWrapProperties.PathLineDict["PathLine"][1],
													RevolutionWrapProperties.PathLineDict["PathLine"][2]),
											new XYZ(RevolutionWrapProperties.PathLineDict["PathLine"][3],
													RevolutionWrapProperties.PathLineDict["PathLine"][4],
													RevolutionWrapProperties.PathLineDict["PathLine"][5]));
			revParams.Axis = axisLine;

			XYZ skOrigin = new XYZ(RevolutionWrapProperties.SketchPlane[0]["SketchOrigin"][0],
								   RevolutionWrapProperties.SketchPlane[0]["SketchOrigin"][1],
								   RevolutionWrapProperties.SketchPlane[0]["SketchOrigin"][2]);
			XYZ skNormal = new XYZ(RevolutionWrapProperties.SketchPlane[1]["SketchNormal"][0],
								   RevolutionWrapProperties.SketchPlane[1]["SketchNormal"][1],
								   RevolutionWrapProperties.SketchPlane[1]["SketchNormal"][2]);
			revParams.SketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(skNormal, skOrigin));


			CurveArrArray curArrArr = new CurveArrArray();
			CurveArray curveArray = new CurveArray();
			int numArr = 1;
			int numLine = 1;
			foreach (var dict in RevolutionWrapProperties.CurveArrArray)
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
			revParams.ProfileCurveArrArray = curArrArr;

			imp.RevolutionCreator revolutionCreator = new imp.RevolutionCreator(docToImport, revParams);
			Element el = revolutionCreator.Create();

			el.SetIdEntityToElement(RevolutionWrapProperties.Id);
		}
	}
}
