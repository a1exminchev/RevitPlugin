using System.Collections.Generic;

namespace Logics.Export{

	public class ProjectDocumentWrap{
		public string                          Name        { get; set; }
		public Dictionary<int, StructuralColumnWrap> StructuralColumns { get; set; }

	}
}
