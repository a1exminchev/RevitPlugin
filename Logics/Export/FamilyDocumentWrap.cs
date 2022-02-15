using System.Collections.Generic;

namespace Logics.Export{

	public class FamilyDocumentWrap{
		public string                          Name        { get; set; }
		
		public Dictionary<int, ExtrusionWrap>  Extrusions  { get; set; }
		public Dictionary<int, RevolutionWrap> Revolutions { get; set; }
		public Dictionary<int, BlendWrap> Blends { get; set; }
		public Dictionary<int, SweepWrap> Sweeps { get; set; }
		public Dictionary<int, SweptBlendWrap> SweptBlends { get; set; }

	}
}
