using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class ExtrusionCreator : AbstractGenericFormCreator<ExtrusionParameters>
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
            Extrusion extrusion = null;
            if (FamDoc != null)
            {
                CurveArrArray curveArrArray = new CurveArrArray();
                curveArrArray.Append(_props.curveArray);

                extrusion = FamDoc.FamilyCreate.NewExtrusion(_props.isSolid, curveArrArray, _props.SketchPlane, _props.Height);
                extrusion.Location.Move(_props.CenterPoint);
            }
            return extrusion;
        }
    }
    public class ExtrusionParameters
    {
        public XYZ CenterPoint { get; set; }
        public bool isSolid { get; set; }
        public CurveArray curveArray { get; set; }
        public double Height { get; set; }
        public SketchPlane SketchPlane { get; set; }
    }
}