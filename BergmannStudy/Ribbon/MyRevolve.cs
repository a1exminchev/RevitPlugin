using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;

namespace StudyTask.Ribbon
{
    public class MyRevolve
    {
        public static void MyRevolveExecute(Document doc)
        {
            XYZ p1 = new XYZ(-10, -10, 10);
            XYZ p2 = new XYZ(-10, 10, 10);
            ModelCurve axis = MakeLine(doc, p1, p2);
            axis.ChangeToReferenceLine();

            ReferenceArray refArray = new ReferenceArray();
            ModelCurve curve1 = MakeLine(doc, new XYZ(0, 0, 10), new XYZ(100, 0, 10));
            ModelCurve curve2 = MakeLine(doc, new XYZ(100, 0, 10), new XYZ(100, 100, 10));
            ModelCurve curve3 = MakeLine(doc, new XYZ(100, 100, 10), new XYZ(0, 0, 10));
            refArray.Append(curve1.GeometryCurve.Reference);
            refArray.Append(curve2.GeometryCurve.Reference);
            refArray.Append(curve3.GeometryCurve.Reference);

            RevolveParameters revolveParameters = new RevolveParameters();
            revolveParameters.isSolid = true;
            revolveParameters.AxisReference = axis.GeometryCurve.Reference;
            revolveParameters.ProfileReferenceArray = refArray;
            revolveParameters.StartingAngle = 0;
            revolveParameters.StartingAngle = Math.PI / 4;

            IFormArrayCreator<RevolveParameters> geometryElementCreator = new RevolveCreator(doc, revolveParameters);
            //FormArray revolve = geometryElementCreator.Create();
            doc.FamilyCreate.NewRevolveForms(true, refArray, axis.GeometryCurve.Reference, 0, Math.PI / 4);
        }
        public static ModelCurve MakeLine(Document doc, XYZ ptA, XYZ ptB)
        {
            Line line = Line.CreateBound(ptA, ptB);
            XYZ norm = ptA.CrossProduct(ptB);
            if (norm.IsZeroLength()) norm = XYZ.BasisZ;
            Plane plane = Plane.CreateByNormalAndOrigin(norm, ptB);
            SketchPlane skplane = SketchPlane.Create(doc, plane);
            ModelCurve modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);
            return modelcurve;
        }
    }
}