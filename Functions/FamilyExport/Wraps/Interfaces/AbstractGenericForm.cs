using Autodesk.Revit.DB;
using Logics.FamilyExport.Wraps.Implementations;

namespace Logics.FamilyExport.Wraps.Interfaces{

	public abstract class AbstractGenericForm : AbstractElementData{
		public AbstractGenericForm(GenericForm el) : base(el) {
			IsSolid = el.IsSolid;
			Visible = el.Visible;

		}

		public AbstractGenericForm() {

		}



		public bool Visible { get; set; }

		public bool IsSolid { get; set; }
	}
}