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
    public class MyBlend
    {
        public static void MyBlendExecute(Document doc)
        {
            SketchPlane bottomSketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, 1)));
            CurveArray baseCurveArray = new CurveArray();
            CurveArray topCurveArray = new CurveArray();

            XYZ p01 = new XYZ(-250 / 304.8, -250 / 304.8, 0);
            XYZ p02 = new XYZ(250 / 304.8, -250 / 304.8, 0);
            XYZ p03 = new XYZ(250 / 304.8, 250 / 304.8, 0);
            XYZ p04 = new XYZ(-250 / 304.8, 250 / 304.8, 0);
            Line line01 = Line.CreateBound(p01, p02);
            Line line02 = Line.CreateBound(p02, p03);
            Line line03 = Line.CreateBound(p03, p04);
            Line line04 = Line.CreateBound(p04, p01);
            Plane topPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, 300 / 304.8));
            Arc arc1 = Arc.Create(topPlane, 100 / 304.8, 0, Math.PI);
            Arc arc2 = Arc.Create(topPlane, 100 / 304.8, Math.PI, 2 * Math.PI);
            baseCurveArray.Append(line01);
            baseCurveArray.Append(line02);
            baseCurveArray.Append(line03);
            baseCurveArray.Append(line04);
            topCurveArray.Append(arc1);
            topCurveArray.Append(arc2);

            BlendParameters blendParameters = new BlendParameters();
            blendParameters.CenterPoint = new XYZ(250 / 304.8, 1000 / 304.8, 0);
            blendParameters.isSolid = true;
            blendParameters.BaseSketchPlane = bottomSketchPlane;
            blendParameters.BaseCurveArray = baseCurveArray;
            blendParameters.TopCurveArray = topCurveArray;
            blendParameters.TopOffset = 450 / 304.8;
            blendParameters.BottomOffset = -450 / 304.8;
            //TaskDialog.Show("Sketch + TopOffset", bottomSketchPlane.GetPlane().Origin.Z.ToString() + " " + blendParameters.TopOffset);
            //TaskDialog.Show("Curve", baseCurveArray.get_Item(0).GetEndPoint(0).ToString() + " " + baseCurveArray.get_Item(3).GetEndPoint(1));
            IGenericFormCreator<BlendParameters> geometryElementCreator = new BlendCreator(doc, blendParameters);
            Blend blend = geometryElementCreator.Create() as Blend;
        }
    }
}