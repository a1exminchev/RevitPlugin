using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using imp = Logics.Geometry.Implementation;
using Logics.Export;

namespace Logics.Import.Transforms
{
    public class SweptBlendTransfer : AbstractTransfer
    {
		public SweptBlendTransfer() {
			
        }
		//Отвечает за создание формы в модели по полученному экземпляру из Json
		public SweptBlendWrapParameters SweptBlendWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			imp.SweptBlendParameters blParams = new imp.SweptBlendParameters();

			blParams.isSolid = SweptBlendWrapProperties.IsSolid;

			XYZ skOrigin = new XYZ(SweptBlendWrapProperties.PathSketchPlane[0]["SketchOrigin"][0],
								   SweptBlendWrapProperties.PathSketchPlane[0]["SketchOrigin"][1],
								   SweptBlendWrapProperties.PathSketchPlane[0]["SketchOrigin"][2]);
			XYZ skNormal = new XYZ(SweptBlendWrapProperties.PathSketchPlane[1]["SketchNormal"][0],
								   SweptBlendWrapProperties.PathSketchPlane[1]["SketchNormal"][1],
								   SweptBlendWrapProperties.PathSketchPlane[1]["SketchNormal"][2]);
			blParams.PathSketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(skNormal, skOrigin));

            Curve path = null;
            foreach (var dict in SweptBlendWrapProperties.PathCurve)
            {
                if (dict.Key == "Line")
                {
                    path = Line.CreateBound(new XYZ(dict.Value[0],
                                                    dict.Value[1],
                                                    dict.Value[2]),
                                            new XYZ(dict.Value[3],
                                                    dict.Value[4],
                                                    dict.Value[5]));
                }
                else if (dict.Key == "Arc")
                {
                    path = Arc.Create(new XYZ(dict.Value[0],
                                            dict.Value[1],
                                            dict.Value[2]),
                                    new XYZ(dict.Value[3],
                                            dict.Value[4],
                                            dict.Value[5]),
                                    new XYZ(dict.Value[6],
                                            dict.Value[7],
                                            dict.Value[8]));
                }
            }
            blParams.PathCurve = path;

            XYZ pr1SkOrigin = new XYZ(SweptBlendWrapProperties.Profile1SketchPlane[0]["SketchOrigin"][0],
                                      SweptBlendWrapProperties.Profile1SketchPlane[0]["SketchOrigin"][1],
                                      SweptBlendWrapProperties.Profile1SketchPlane[0]["SketchOrigin"][2]);
            XYZ pr1SkNormal = new XYZ(SweptBlendWrapProperties.Profile1SketchPlane[1]["SketchNormal"][0],
                                      SweptBlendWrapProperties.Profile1SketchPlane[1]["SketchNormal"][1],
                                      SweptBlendWrapProperties.Profile1SketchPlane[1]["SketchNormal"][2]);
            blParams.Profile1SketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(pr1SkNormal, pr1SkOrigin));
            XYZ pr2SkOrigin = new XYZ(SweptBlendWrapProperties.Profile2SketchPlane[0]["SketchOrigin"][0],
                                      SweptBlendWrapProperties.Profile2SketchPlane[0]["SketchOrigin"][1],
                                      SweptBlendWrapProperties.Profile2SketchPlane[0]["SketchOrigin"][2]);
            XYZ pr2SkNormal = new XYZ(SweptBlendWrapProperties.Profile2SketchPlane[1]["SketchNormal"][0],
                                      SweptBlendWrapProperties.Profile2SketchPlane[1]["SketchNormal"][1],
                                      SweptBlendWrapProperties.Profile2SketchPlane[1]["SketchNormal"][2]);
            blParams.Profile2SketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(pr2SkNormal, pr2SkOrigin));
            Transform t1 = Transform.CreateTranslation(new XYZ(0 - blParams.PathSketchPlane.GetPlane().Origin.X,
                0 - blParams.PathSketchPlane.GetPlane().Origin.Y,
                DeltaModul(blParams.Profile1SketchPlane.GetPlane().Origin.Z, blParams.Profile1SketchPlane.GetPlane().Origin.Z))); //Bot
            Transform t2 = Transform.CreateTranslation(new XYZ(0 - blParams.PathSketchPlane.GetPlane().Origin.X,
                0 - blParams.PathSketchPlane.GetPlane().Origin.Y,
                DeltaModul(blParams.Profile2SketchPlane.GetPlane().Origin.Z, blParams.Profile2SketchPlane.GetPlane().Origin.Z))); //Top

            CurveArray curveArray1 = new CurveArray();
            int numLine = 1;
            foreach (var dict in SweptBlendWrapProperties.Profile1CurveArray)
            {
                if (dict.FirstOrDefault().Key.Trim('L', 'i', 'n', 'e') == numLine.ToString())
                {
                    curveArray1.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
                                                                  dict.FirstOrDefault().Value[1],
                                                                  dict.FirstOrDefault().Value[2]),
                                                          new XYZ(dict.FirstOrDefault().Value[3],
                                                                  dict.FirstOrDefault().Value[4],
                                                                  dict.FirstOrDefault().Value[5])).CreateTransformed(t1));
                    numLine += 1;
                }
                else if (dict.FirstOrDefault().Key.Trim('A', 'r', 'c') == numLine.ToString())
                {
                    curveArray1.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
                                                             dict.FirstOrDefault().Value[1],
                                                             dict.FirstOrDefault().Value[2]),
                                                     new XYZ(dict.FirstOrDefault().Value[3],
                                                             dict.FirstOrDefault().Value[4],
                                                             dict.FirstOrDefault().Value[5]),
                                                     new XYZ(dict.FirstOrDefault().Value[6],
                                                             dict.FirstOrDefault().Value[7],
                                                             dict.FirstOrDefault().Value[8])).CreateTransformed(t1));
                    numLine += 1;
                }
            }
            blParams.Profile1CurveArray = curveArray1;

            CurveArray curveArray2 = new CurveArray();
            numLine = 1;
            foreach (var dict in SweptBlendWrapProperties.Profile2CurveArray)
            {
                if (dict.FirstOrDefault().Key.Split('e')[1] == numLine.ToString())
                {
                    curveArray2.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
                                                                  dict.FirstOrDefault().Value[1],
                                                                  dict.FirstOrDefault().Value[2]),
                                                          new XYZ(dict.FirstOrDefault().Value[3],
                                                                  dict.FirstOrDefault().Value[4],
                                                                  dict.FirstOrDefault().Value[5])).CreateTransformed(t2));
                    numLine += 1;
                }
                else if (dict.FirstOrDefault().Key.Split('c')[1] == numLine.ToString())
                {
                    curveArray2.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
                                                             dict.FirstOrDefault().Value[1],
                                                             dict.FirstOrDefault().Value[2]),
                                                     new XYZ(dict.FirstOrDefault().Value[3],
                                                             dict.FirstOrDefault().Value[4],
                                                             dict.FirstOrDefault().Value[5]),
                                                     new XYZ(dict.FirstOrDefault().Value[6],
                                                             dict.FirstOrDefault().Value[7],
                                                             dict.FirstOrDefault().Value[8])).CreateTransformed(t2));
                    numLine += 1;
                }
            }
            blParams.Profile2CurveArray = curveArray2;

            imp.SweptBlendCreator sweptBlendCreator = new imp.SweptBlendCreator(docToImport, blParams);
            sweptBlendCreator.Create();
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
