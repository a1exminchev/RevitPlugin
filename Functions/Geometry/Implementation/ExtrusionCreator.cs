using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class ExtrusionCreator : AbstractGeometryElementCreator<ExtrusionParameters>
    {
        protected readonly Document FamDoc;
        private readonly ExtrusionParameters _props;


        public ExtrusionCreator(Document doc, ExtrusionParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            Extrusion rectExtrusion = null;
            if (FamDoc != null)
            {
                CurveArrArray curveArrArray = new CurveArrArray();
                CurveArray curveArray1 = new CurveArray();
                XYZ p0 = XYZ.Zero;
                XYZ p1 = new XYZ(_props.Width, 0, 0);
                XYZ p2 = new XYZ(_props.Width, _props.Width, 0);
                XYZ p3 = new XYZ(0, _props.Width, 0);
                Line line1 = Line.CreateBound(p0, p1);
                Line line2 = Line.CreateBound(p1, p2);
                Line line3 = Line.CreateBound(p2, p3);
                Line line4 = Line.CreateBound(p3, p0);
                curveArray1.Append(line1);
                curveArray1.Append(line2);
                curveArray1.Append(line3);
                curveArray1.Append(line4);
                curveArrArray.Append(curveArray1);

                rectExtrusion = FamDoc.FamilyCreate.NewExtrusion(true, curveArrArray, _props.SketchPlane, _props.Height);
                rectExtrusion.Location.Move(_props.CenterPoint);
                XYZ transPoint1 = new XYZ(-_props.Width/2, -_props.Width / 2, 0);
                ElementTransformUtils.MoveElement(FamDoc, rectExtrusion.Id, transPoint1);
            }
            return rectExtrusion;
        }
    }
    public class ExtrusionParameters
    {
        public XYZ CenterPoint { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public SketchPlane SketchPlane { get; set; }
    }
}
