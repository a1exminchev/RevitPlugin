using Autodesk.Revit.DB;
using Logics.FamilyExport.Wraps.Implementations;

namespace Logics.FamilyExport.Wraps.Interfaces{

	public abstract class AbstractGenericForm : AbstractElementData
	{
		public bool Visible { get; set; }
		public bool	IsSolid { get; set; }
		public AbstractGenericForm(GenericForm gen) : base(gen) {

			IsSolid = gen.IsSolid;
			Visible = gen.Visible;
		}

		public AbstractGenericForm() {
			
		}
	}
}