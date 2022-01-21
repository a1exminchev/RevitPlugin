using Autodesk.Revit.DB;
using Logics.FamilyExport.Wraps.Interfaces;

namespace Logics.FamilyExport{


	public class ExtrusionWrap : AbstractGenericForm{
		public ExtrusionWrap(Extrusion el) : base(el) {
		}

		public ExtrusionWrap() {

		}
	}
}