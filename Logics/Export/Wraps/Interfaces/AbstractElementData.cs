using Autodesk.Revit.DB;
using Logics.Export.Wraps.Interfaces;

namespace Logics.Export.Wraps.Implementations{
	public abstract class AbstractElementData : IElement
	{
		public Document doc { get; set; }
		public int             Id       { get; set; }
		public string          Name     { get; set; }
		public string          Type     { get; set; }
		public bool			   isPinned { get; set; }
		public string		   Category { get; set; }
		public AbstractElementData() {
		}

		public AbstractElementData(Element el) {
			Id = el.Id.IntegerValue;
			Name = el.Name;
			Type = el.GetType().ToString();
			isPinned = el.Pinned;
			Category = el.Category?.ToString();
			doc = el.Document;
		}
	}
}