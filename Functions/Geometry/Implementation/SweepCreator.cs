using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class SweepCreator : AbstractGenericFormCreator<SweepParameters>
    {
        protected readonly Document FamDoc;
        private readonly SweepParameters _props;


        public SweepCreator(Document doc, SweepParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            Sweep sweep = null;
            if (FamDoc != null)
            {
                SweepProfile profile = FamDoc.Application.Create.NewCurveLoopsProfile(_props.ProfileCurveArrArray) as SweepProfile;
                sweep = FamDoc.FamilyCreate.NewSweep(_props.isSolid, _props.PathCurveArray, _props.PathSketchPlane, profile, _props.WhichPathLineIsForProfile, ProfilePlaneLocation.Start);
                if (_props.CenterPoint != null)
                {
                    sweep.Location.Move(_props.CenterPoint);
                }
            }
            return sweep;
        }
    }
    public class SweepParameters
    {
        public XYZ CenterPoint { get; set; }
        public bool isSolid { get; set; }
        public CurveArray PathCurveArray { get; set; }
        public CurveArrArray ProfileCurveArrArray { get; set; }
        public int WhichPathLineIsForProfile { get; set; }
        public SketchPlane PathSketchPlane { get; set; }
    }
}