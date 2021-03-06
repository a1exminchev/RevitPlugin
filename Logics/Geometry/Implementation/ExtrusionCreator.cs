using Autodesk.Revit.DB;
using Logics.Geometry.Interface;

namespace Logics.Geometry.Implementation{
	public class ExtrusionCreator : AbstractGenericFormCreator<ExtrusionParameters>
	{
		protected readonly Document            FamDoc;
		private readonly   ExtrusionParameters _props;

		public ExtrusionCreator(Document doc, ExtrusionParameters properties) : base(doc, properties)
		{
			if (doc.IsFamilyDocument) {
				FamDoc = doc;
			}

			_props = properties;
		}

		public override GenericForm Create()
		{
			Extrusion extrusion = null;
			if (FamDoc != null) {
				extrusion = FamDoc.FamilyCreate.NewExtrusion(_props.isSolid
				                                             , _props.curveArrArray
				                                             , _props.SketchPlane
				                                             , _props.EndOffset);
				extrusion.StartOffset = _props.StartOffset;
				if (_props.CenterPoint != null) {
					extrusion.Location.Move(_props.CenterPoint);
				}
			}
			return extrusion;
		}
	}

	public class ExtrusionParameters{
		public XYZ           CenterPoint { get; set; }
		public bool          isSolid     { get; set; }
		public CurveArrArray curveArrArray  { get; set; }
		public double        EndOffset      { get; set; }
		public double		 StartOffset { get; set; }
		public SketchPlane   SketchPlane { get; set; }
	}
}