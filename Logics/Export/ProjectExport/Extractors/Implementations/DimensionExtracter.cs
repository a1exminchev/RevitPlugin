using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.Export.ModelExport.Extractors.Implementations
{
	public class DimensionExtracter : AbstractExtractor<DimensionWrap>
	{
		public DimensionExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, DimensionWrap> ExtractWork()
		{
			var retl = new Dictionary<int, DimensionWrap>();
			var elements = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Dimensions)
															 .OfClass(typeof(Dimension))
															 .OfType<Dimension>();
															
			foreach (var elem in elements)
			{
				var dim = new DimensionWrap(elem);
				retl[dim.Id] = dim;
			}

			return retl;
		}
	}
}