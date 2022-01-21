using Autodesk.Revit.DB;

namespace Logics.FamilyExport.Wraps.Interfaces{

	public interface IElement{
		int             Id       { get; set; }
		string          Name     { get; set; }
		BuiltInCategory Category { get; set; }
		string          Type     { get; set; }
	}
}