using Autodesk.Revit.DB;
using Logics.Export.Wraps.Implementations;

namespace Logics.Export.Wraps.Interfaces{

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