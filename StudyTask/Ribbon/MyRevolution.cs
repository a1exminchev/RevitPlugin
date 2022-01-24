using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;

namespace StudyTask.Ribbon
{
    public class MyRevolution
    {
        public static void MyRevolutionExecute(Document doc)
        {
            SketchPlane sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero)); //XY plane

            XYZ p1 = new XYZ(-10, -10, 0);
            XYZ p2 = new XYZ(-10, 10, 0);
            Line axis = Line.CreateBound(p1, p2);

            CurveArrArray curArrArray = new CurveArrArray();
            CurveArray curArr = new CurveArray();
            Line line1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0));
            Line line2 = Line.CreateBound(new XYZ(1, 0, 0), new XYZ(1, 1, 0));
            Line line3 = Line.CreateBound(new XYZ(1, 1, 0), new XYZ(0, 0, 0));
            curArr.Append(line1);
            curArr.Append(line2);
            curArr.Append(line3);
            curArrArray.Append(curArr);

            RevolutionParameters revolutionParameters = new RevolutionParameters();
            revolutionParameters.isSolid = true;
            revolutionParameters.Axis= axis;
            revolutionParameters.SketchPlane = sketchPlane;
            revolutionParameters.ProfileCurveArrArray = curArrArray;
            revolutionParameters.StartingAngle = 0;
            revolutionParameters.EndingAngle = Math.PI / 4;

            IGenericFormCreator<RevolutionParameters> geometryElementCreator = new RevolutionCreator(doc, revolutionParameters);
            Revolution revolution = geometryElementCreator.Create() as Revolution;
        }
    }
}