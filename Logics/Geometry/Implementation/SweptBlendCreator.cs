using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class SweptBlendCreator : AbstractGenericFormCreator<SweptBlendParameters>
    {
        protected readonly Document FamDoc;
        private readonly SweptBlendParameters _props;


        public SweptBlendCreator(Document doc, SweptBlendParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            SweptBlend swept = null;
            if (FamDoc != null)
            {
                CurveArrArray curveArrArray1 = new CurveArrArray();
                CurveArrArray curveArrArray2 = new CurveArrArray();
                curveArrArray1.Append(_props.Profile1CurveArray);
                curveArrArray2.Append(_props.Profile2CurveArray);
                SweepProfile profile1 = FamDoc.Application.Create.NewCurveLoopsProfile(curveArrArray1) as SweepProfile;
                SweepProfile profile2 = FamDoc.Application.Create.NewCurveLoopsProfile(curveArrArray2) as SweepProfile;
                try
                {
                    swept = FamDoc.FamilyCreate.NewSweptBlend(_props.isSolid, _props.PathCurve, _props.PathSketchPlane, profile1, profile2);
                }
                catch { }
            }
            return swept;
        }
    }
    public class SweptBlendParameters
    {
        public Curve PathCurve { get; set; }
        public SketchPlane PathSketchPlane { get; set; }
        public bool isSolid { get; set; }
        public CurveArray Profile1CurveArray { get; set; }
        public CurveArray Profile2CurveArray { get; set; }
        public SketchPlane Profile1SketchPlane { get; set; }
        public SketchPlane Profile2SketchPlane { get; set; }
    }
}