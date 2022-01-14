using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation
{
    public class RevolveCreator : AbstractFormArrayCreator<RevolveParameters>
    {
        protected readonly Document FamDoc;
        private readonly RevolveParameters _props;


        public RevolveCreator(Document doc, RevolveParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override FormArray Create()
        {
            FormArray revolve = null;
            if (FamDoc != null)
            {
                revolve = FamDoc.FamilyCreate.NewRevolveForms(_props.isSolid, _props.ProfileReferenceArray, _props.AxisReference, _props.StartingAngle, _props.EndingAngle);
                //тут ошибка
            }
            return revolve;
        }
    }
    public class RevolveParameters
    {
        public bool isSolid { get; set; }
        public Reference AxisReference { get; set; }
        public ReferenceArray ProfileReferenceArray { get; set; }
        public double StartingAngle { get; set; }
        public double EndingAngle { get; set; }
    }
}