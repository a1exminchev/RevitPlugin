using Autodesk.Revit.DB;

namespace Logics.Export.Wraps.Interfaces{

	public interface IElement
	{
		int         Id			{ get; set; }
		string      Name		{ get; set; }
		string      Type		{ get; set; }
		bool		isPinned	{ get; set; }
		string		Category	{ get; set; }
	}
}