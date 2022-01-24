using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.FamilyExport.ModelExport.Extractors.Implementations
{

	public class SweptBlendExtracter : AbstractExtractor<SweptBlendWrap>
	{
		public SweptBlendExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, SweptBlendWrap> ExtractWork()
		{
			var retl = new Dictionary<int, SweptBlendWrap>();
			var elements = new FilteredElementCollector(_doc).OfClass(typeof(SweptBlend))
															 .OfType<SweptBlend>();
			foreach (var elem in elements)
			{
				var swept = new SweptBlendWrap(elem);
				retl[swept.Id] = swept;
			}

			return retl;
		}
	}
}