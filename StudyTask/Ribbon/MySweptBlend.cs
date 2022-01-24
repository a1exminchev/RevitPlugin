using System;
using System.Collections.Generic;
using System.Linq;
using Logics.Geometry;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;
using Autodesk.Revit.DB.Structure;

namespace StudyTask.Ribbon
{
    public class MySweptBlend
    {
        public static void MySweptBlendBadExecute(Document doc)
        {
            SketchPlane pathSketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero)); //XZ plane

            XYZ p1 = new XYZ(0, 0, 0);
            XYZ p2 = new XYZ(0, 500 / 304.8, 0);
            Curve path = Line.CreateBound(p1, p2);
            Line line1 = Line.CreateBound(new XYZ(1, 0, 0), new XYZ(1, 1, 0));
            Line line2 = Line.CreateBound(new XYZ(1, 1, 0), new XYZ(-2, -1, 0));
            Line line3 = Line.CreateBound(new XYZ(-2, -1, 0), new XYZ(1, 0, 0));

            Plane profilePlane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero); //XZ plane
            CurveArray profile1CurveArray = new CurveArray();
            CurveArray profile2CurveArray = new CurveArray();
            Arc arc1 = Arc.Create(profilePlane, 3, 0, Math.PI);
            Arc arc2 = Arc.Create(profilePlane, 3, Math.PI, Math.PI * 2);

            profile1CurveArray.Append(arc1);
            profile1CurveArray.Append(arc2);
            profile2CurveArray.Append(line1);
            profile2CurveArray.Append(line2);
            profile2CurveArray.Append(line3);

            SweptBlendParameters sweptParameters = new SweptBlendParameters();
            sweptParameters.isSolid = true;
            sweptParameters.PathCurve = path;
            sweptParameters.PathSketchPlane = pathSketchPlane;
            sweptParameters.Profile1CurveArray = profile1CurveArray;
            sweptParameters.Profile2CurveArray = profile2CurveArray;
            IGenericFormCreator<SweptBlendParameters> geometryElementCreator = new SweptBlendCreator(doc, sweptParameters);
            SweptBlend swept = geometryElementCreator.Create() as SweptBlend;
        }
        public static void MySweptBlendExecute(Document doc)
        {
            SketchPlane pathSketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero)); //XZ plane

            XYZ p1 = new XYZ(0, 0, 0);
            XYZ p2 = new XYZ(0, 0, 1000 / 304.8);
            Curve path = Line.CreateBound(p1, p2);

            Line line1 = Line.CreateBound(new XYZ(1, 0, 0), new XYZ(1, 1, 0));
            Line line2 = Line.CreateBound(new XYZ(1, 1, 0), new XYZ(-2, -1, 0));
            Line line3 = Line.CreateBound(new XYZ(-2, -1, 0), new XYZ(1, 0, 0));

            Plane profilePlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero); //XY plane
            CurveArray profile1CurveArray = new CurveArray();
            CurveArray profile2CurveArray = new CurveArray();
            Arc arc1 = Arc.Create(profilePlane, 1, 0, Math.PI);
            Arc arc2 = Arc.Create(profilePlane, 1, Math.PI, Math.PI * 2);

            profile1CurveArray.Append(arc1);
            profile1CurveArray.Append(arc2);
            profile2CurveArray.Append(line1);
            profile2CurveArray.Append(line2);
            profile2CurveArray.Append(line3);

            SweptBlendParameters sweptParameters = new SweptBlendParameters();
            sweptParameters.isSolid = true;
            sweptParameters.PathCurve = path;
            sweptParameters.PathSketchPlane = pathSketchPlane;
            sweptParameters.Profile1CurveArray = profile1CurveArray;
            sweptParameters.Profile2CurveArray = profile2CurveArray;
            IGenericFormCreator<SweptBlendParameters> geometryElementCreator = new SweptBlendCreator(doc, sweptParameters);
            SweptBlend swept = geometryElementCreator.Create() as SweptBlend;
            //Line axis = Line.CreateBound(XYZ.Zero, XYZ.BasisX);
            //ElementTransformUtils.RotateElement(doc, swept.Id, axis, -Math.PI / 3); //Rotate around X axis 60 degree counterclockwise
        }
        //https://spiderinnet.typepad.com/blog/2014/03/revisit-sweep-creation-using-revit-family-net-api.html
    }
}