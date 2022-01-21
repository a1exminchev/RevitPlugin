using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Logics.FamilyExport{

	public class FamilyDocumentWrap{
		public string                          Name        { get; set; }
		public BuiltInCategory                 Category    { get; set; }
		public Dictionary<int, ExtrusionWrap>  Extrusions  { get; set; }
		public Dictionary<int, RevolutionWrap> Revolutions { get; set; }


	}
}