using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Logics.Export;
using imp = Logics.Geometry.Implementation;

namespace Logics.Import.Transforms
{
    public class SweepTransfer : AbstractTransfer
    {
		public SweepTransfer() {
			
        }
		//Отвечает за создание формы в модели по полученному экземпляру из Json
		public SweepWrapParameters SweepWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			imp.SweepParameters swParams = new imp.SweepParameters();

			swParams.isSolid = SweepWrapProperties.IsSolid;

			CurveArray pathArray = new CurveArray();
            int pathNumLine = 1;
            foreach (var dict in SweepWrapProperties.PathCurveArray)
            {
                if (dict.FirstOrDefault().Key.Trim('L', 'i', 'n', 'e') == pathNumLine.ToString())
                {
                    pathArray.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
                                                                  dict.FirstOrDefault().Value[1],
                                                                  dict.FirstOrDefault().Value[2]),
                                                          new XYZ(dict.FirstOrDefault().Value[3],
                                                                  dict.FirstOrDefault().Value[4],
                                                                  dict.FirstOrDefault().Value[5])));
                    pathNumLine += 1;
                }
                else if (dict.FirstOrDefault().Key.Trim('A', 'r', 'c') == pathNumLine.ToString())
                {
                    pathArray.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
                                                             dict.FirstOrDefault().Value[1],
                                                             dict.FirstOrDefault().Value[2]),
                                                     new XYZ(dict.FirstOrDefault().Value[3],
                                                             dict.FirstOrDefault().Value[4],
                                                             dict.FirstOrDefault().Value[5]),
                                                     new XYZ(dict.FirstOrDefault().Value[6],
                                                             dict.FirstOrDefault().Value[7],
                                                             dict.FirstOrDefault().Value[8])));
                    pathNumLine += 1;
                }
            }
            swParams.PathCurveArray = pathArray;

            XYZ skOrigin = new XYZ(SweepWrapProperties.PathSketchPlane[0]["SketchOrigin"][0],
                                   SweepWrapProperties.PathSketchPlane[0]["SketchOrigin"][1],
                                   SweepWrapProperties.PathSketchPlane[0]["SketchOrigin"][2]);
            XYZ skNormal = new XYZ(SweepWrapProperties.PathSketchPlane[1]["SketchNormal"][0],
                                   SweepWrapProperties.PathSketchPlane[1]["SketchNormal"][1],
                                   SweepWrapProperties.PathSketchPlane[1]["SketchNormal"][2]);
            swParams.PathSketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(skNormal, skOrigin));


            CurveArrArray curArrArr = new CurveArrArray();
            CurveArray curveArray = new CurveArray();
            int numArr = 1;
            int numLine = 1;
            XYZ prSkOrigin = new XYZ(SweepWrapProperties.ProfileSketchPlane[0]["SketchOrigin"][0],
                                   SweepWrapProperties.ProfileSketchPlane[0]["SketchOrigin"][1],
                                   SweepWrapProperties.ProfileSketchPlane[0]["SketchOrigin"][2]);
            XYZ prSkNormal = new XYZ(SweepWrapProperties.ProfileSketchPlane[1]["SketchNormal"][0],
                                   SweepWrapProperties.ProfileSketchPlane[1]["SketchNormal"][1],
                                   SweepWrapProperties.ProfileSketchPlane[1]["SketchNormal"][2]);
            swParams.ProfileSketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(prSkNormal, prSkOrigin));
            Transform tProfile = Transform.CreateTranslation(new XYZ(0 - swParams.PathSketchPlane.GetPlane().Origin.X,
                                                                     0 - swParams.PathSketchPlane.GetPlane().Origin.Y,
                                                                     DeltaModul(swParams.ProfileSketchPlane.GetPlane().Origin.Z,
                                                                                swParams.ProfileSketchPlane.GetPlane().Origin.Z)));
            foreach (var dict in SweepWrapProperties.ProfileCurveArrArray)
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
                                                                   dict.FirstOrDefault().Value[5])).CreateTransformed(tProfile));
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
                                                             dict.FirstOrDefault().Value[8])).CreateTransformed(tProfile));

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
                                                                   dict.FirstOrDefault().Value[5])).CreateTransformed(tProfile));
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
                                                             dict.FirstOrDefault().Value[8])).CreateTransformed(tProfile));
                    }
                    numLine = 2;
                }
            }
            curArrArr.Append(curveArray);
            swParams.ProfileCurveArrArray = curArrArr;

            imp.SweepCreator sweepCreator = new imp.SweepCreator(docToImport, swParams);
            try
            {
                sweepCreator.Create();
            }
            catch { }
        }
        private double DeltaModul(double elevation, double delta)
        {
            double _delta = 0;
            if (elevation > 0 && elevation - delta >= 0)
            {
                _delta = -delta;
                return _delta;
            }

            if (elevation < 0 && elevation + delta <= 0)
            {
                _delta = -delta;
                return _delta;
            }

            if (elevation > 0 && elevation - delta < 0)
            {
                _delta = elevation - delta;
                return _delta;
            }

            if (elevation < 0 && elevation + delta > 0)
            {
                _delta = -elevation + delta;
                return _delta;
            }

            return _delta;
        }
    }
}
