using System;
using System.Collections.Generic;
using System.Linq;
using Logics.Geometry;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;

namespace StudyTask.Ribbon
{
    public class MySweep
    {
        public static void MySweepBadExecute(Document doc)
        {
            Plane profilePlane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero); //XZ plane
            SketchPlane pathPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero)); //XY plane
            CurveArray pathCurveArray = new CurveArray();
            XYZ p1 = new XYZ(0, 0, 0);
            XYZ p2 = new XYZ(0, 500 / 304.8, 0);
            Line line1 = Line.CreateBound(p1, p2);
            //Line line2 = Line.CreateBound(p2, p3);
            CurveArrArray profileCurveArrArray = new CurveArrArray();
            CurveArray profileCurveArray = new CurveArray();
            Arc arc1 = Arc.Create(profilePlane, 3, 0, Math.PI);
            Arc arc2 = Arc.Create(profilePlane, 3, Math.PI, Math.PI * 2);

            profileCurveArray.Append(arc1);
            profileCurveArray.Append(arc2);
            profileCurveArrArray.Append(profileCurveArray);

            pathCurveArray.Append(line1);
            //pathCurveArray.Append(line2);

            SweepParameters sweepParameters = new SweepParameters();
            sweepParameters.isSolid = true;
            sweepParameters.PathSketchPlane = pathPlane;
            sweepParameters.PathCurveArray = pathCurveArray;
            sweepParameters.WhichPathLineIsForProfile = 0;
            sweepParameters.ProfileCurveArrArray = profileCurveArrArray;
            IGenericFormCreator<SweepParameters> geometryElementCreator = new SweepCreator(doc, sweepParameters);
            Sweep sweep = geometryElementCreator.Create() as Sweep;
            //doc.FamilyCreate.NewSweep(true, pathCurveArray, pathPlane, profileCurveArray, 0, ProfilePlaneLocation.Start);
        }
        public static void MySweepExecute(Document doc)
        {
            //Plane profilePlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero); //XY plane
            SketchPlane pathPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero)); //XZ plane
            CurveArray pathCurveArray = new CurveArray();
            XYZ p1 = new XYZ(0, 0, 0);
            XYZ p2 = new XYZ(0, 0, 1000 / 304.8);
            XYZ p3 = new XYZ(2000 / 304.8, 0, 1000 / 304.8);
            Line line1 = Line.CreateBound(p1, p2);
            Line line2 = Line.CreateBound(p2, p3);
            CurveArrArray profileCurveArrArray = new CurveArrArray();
            CurveArray profileCurveArray = new CurveArray();
            Line line01 = Line.CreateBound(new XYZ(1, 0, 0), new XYZ(1, 1, 0));
            Line line02 = Line.CreateBound(new XYZ(1, 1, 0), new XYZ(-2, -1, 0));
            Line line03 = Line.CreateBound(new XYZ(-2, -1, 0), new XYZ(1, 0, 0));
            //Arc arc1 = Arc.Create(profilePlane, 1, 0, Math.PI);
            //Arc arc2 = Arc.Create(profilePlane, 1, Math.PI, Math.PI * 2);

            profileCurveArray.Append(line01);
            profileCurveArray.Append(line02);
            profileCurveArray.Append(line03);
            profileCurveArrArray.Append(profileCurveArray);

            pathCurveArray.Append(line1);
            pathCurveArray.Append(line2);

            SweepParameters sweepParameters = new SweepParameters();
            sweepParameters.isSolid = true;
            sweepParameters.PathSketchPlane = pathPlane;
            sweepParameters.PathCurveArray = pathCurveArray;
            sweepParameters.WhichPathLineIsForProfile = 0;
            sweepParameters.ProfileCurveArrArray = profileCurveArrArray;
            //sweepParameters.AngleFromXZtoY = - Math.PI / 5; //Rotate around X axis 36 degree counterclockwise
            //sweepParameters.AngleFromYZtoX = Math.PI / 3; //Rotate around Y axis 60 degree clockwise
            IGenericFormCreator<SweepParameters> geometryElementCreator = new SweepCreator(doc, sweepParameters);
            Sweep sweep = geometryElementCreator.Create() as Sweep;
        }
        //https://spiderinnet.typepad.com/blog/2014/03/revisit-sweep-creation-using-revit-family-net-api.html
    }
}