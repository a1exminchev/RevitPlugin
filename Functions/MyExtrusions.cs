using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace Logics
{
    public class MyExtrusions
    {
		public static Extrusion CreateCubeExtrusion(Document doc, SketchPlane sketchPlane)
		{
			Extrusion rectExtrusion = null;
			if (true == doc.IsFamilyDocument)
			{
				CurveArrArray curveArrArray = new CurveArrArray();
				CurveArray curveArray1 = new CurveArray();
				XYZ p0 = XYZ.Zero;
				XYZ p1 = new XYZ(500 / 304.8, 0, 0);
				XYZ p2 = new XYZ(500 / 304.8, 500 / 304.8, 0);
				XYZ p3 = new XYZ(0, 500 / 304.8, 0);
				Line line1 = Line.CreateBound(p0, p1);
				Line line2 = Line.CreateBound(p1, p2);
				Line line3 = Line.CreateBound(p2, p3);
				Line line4 = Line.CreateBound(p3, p0);
				curveArray1.Append(line1);
				curveArray1.Append(line2);
				curveArray1.Append(line3);
				curveArray1.Append(line4);
				curveArrArray.Append(curveArray1);

				

				rectExtrusion = doc.FamilyCreate.NewExtrusion(true, curveArrArray, sketchPlane, 500 / 304.8);
				XYZ transPoint1 = new XYZ(-250 / 304.8, -250 / 304.8, 0);
				ElementTransformUtils.MoveElement(doc, rectExtrusion.Id, transPoint1);
			}
			return rectExtrusion;
		}
	}
}