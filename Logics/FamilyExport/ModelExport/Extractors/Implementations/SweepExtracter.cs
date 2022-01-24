using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.FamilyExport.ModelExport.Extractors.Implementations
{

	public class SweepExtracter : AbstractExtractor<SweepWrap>
	{
		public SweepExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, SweepWrap> ExtractWork()
		{
			var retl = new Dictionary<int, SweepWrap>();
			var elements = new FilteredElementCollector(_doc).OfClass(typeof(Sweep))
															 .OfType<Sweep>();
			foreach (var elem in elements)
			{
				var sweep = new SweepWrap(elem);
				retl[sweep.Id] = sweep;
			}

			return retl;
		}
	}
}