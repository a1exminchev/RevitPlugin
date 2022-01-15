using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class RevolutionCreator : AbstractGenericFormCreator<RevolutionParameters>
    {
        protected readonly Document FamDoc;
        private readonly RevolutionParameters _props;


        public RevolutionCreator(Document doc, RevolutionParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            GenericForm revolution = null;
            if (FamDoc != null)
            {
                revolution = FamDoc.FamilyCreate.NewRevolution(_props.isSolid, _props.ProfileCurveArrArray, _props.SketchPlane, _props.Axis, _props.StartingAngle, _props.EndingAngle);
            }
            return revolution;
        }
    }
    public class RevolutionParameters
    {
        public bool isSolid { get; set; }
        public CurveArrArray ProfileCurveArrArray { get; set; }
        public SketchPlane SketchPlane { get; set; }
        public Line Axis { get; set; }
        public double StartingAngle { get; set; }
        public double EndingAngle { get; set; }
    }
}