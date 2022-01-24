using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.FamilyExport.ModelExport.Extractors.Implementations
{

	public class RevolutionExtracter : AbstractExtractor<RevolutionWrap>
	{
		public RevolutionExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, RevolutionWrap> ExtractWork()
		{
			var retl = new Dictionary<int, RevolutionWrap>();
			var elements = new FilteredElementCollector(_doc).OfClass(typeof(Revolution))
															 .OfType<Revolution>();
			foreach (var elem in elements)
			{
				var revolution = new RevolutionWrap(elem);
				retl[revolution.Id] = revolution;
			}

			return retl;
		}
	}
}