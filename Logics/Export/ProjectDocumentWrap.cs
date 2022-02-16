using System.Collections.Generic;

namespace Logics.Export{

	public class ProjectDocumentWrap{
		public string                          Name        { get; set; }
		public Dictionary<int, StructuralColumnWrap> StructuralColumns { get; set; }
		public Dictionary<int, ColumnWrap> Columns { get; set; }
		public Dictionary<int, BeamWrap> Beams { get; set; }

	}
}
