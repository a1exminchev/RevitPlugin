using Autodesk.Revit.DB;
using Logics.FamilyExport.Wraps.Interfaces;

namespace Logics.FamilyExport.Wraps.Implementations{
	public abstract class AbstractElementData : IElement{
		public int             Id       { get; set; }
		public string          Name     { get; set; }
		public BuiltInCategory Category { get; set; }
		public string          Type     { get; set; }

		public AbstractElementData() {
		}

		public AbstractElementData(Element el) {
			Id = el.Id.IntegerValue;
		}
	}
}