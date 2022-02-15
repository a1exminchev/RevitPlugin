using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.Export.ModelExport.Extractors.Implementations
{

	public class BlendExtracter : AbstractExtractor<BlendWrap>
	{
		public BlendExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, BlendWrap> ExtractWork()
		{
			var retl = new Dictionary<int, BlendWrap>();
			var elements = new FilteredElementCollector(_doc).OfClass(typeof(Blend))
															 .OfType<Blend>();
			foreach (var elem in elements)
			{
				var blend = new BlendWrap(elem);
				retl[blend.Id] = blend;
			}

			return retl;
		}
	}
}