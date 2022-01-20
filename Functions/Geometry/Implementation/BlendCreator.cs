using Autodesk.Revit.DB;
using Logics.Geometry.Interface;
using System.Linq;

namespace Logics.Geometry.Implementation
{
    public class BlendCreator : AbstractGenericFormCreator<BlendParameters>
    {
        protected readonly Document FamDoc;
        private readonly BlendParameters _props;


        public BlendCreator(Document doc, BlendParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            Blend blend = null;
            if (FamDoc != null)
            {
                blend = FamDoc.FamilyCreate.NewBlend(_props.isSolid, _props.TopCurveArray, _props.BaseCurveArray, _props.BaseSketchPlane);
                blend.get_Parameter(BuiltInParameter.BLEND_END_PARAM).Set(_props.TopOffset);
                blend.get_Parameter(BuiltInParameter.BLEND_START_PARAM).Set(_props.BottomOffset);
                if (_props.CenterPoint != null)
                {
                    blend.Location.Move(_props.CenterPoint);
                }
            }
            return blend;
        }
    }
    public class BlendParameters
    {
        public XYZ CenterPoint { get; set; }
        public bool isSolid { get; set; }
        public CurveArray BaseCurveArray { get; set; }
        public CurveArray TopCurveArray { get; set; }
        public SketchPlane BaseSketchPlane { get; set; }
        public double TopOffset { get; set; }
        public double BottomOffset { get; set; }
    }
}