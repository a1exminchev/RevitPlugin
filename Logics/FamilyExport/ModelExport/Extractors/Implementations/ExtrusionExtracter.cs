using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.FamilyExport.ModelExport.Extractors.Implementations
{
	public class ExtrusionExtracter : AbstractExtractor<ExtrusionWrap>
	{
		public ExtrusionExtracter(Document document) : base(document)
		{
		}

		public override Dictionary<int, ExtrusionWrap> ExtractWork()
		{
			var retl = new Dictionary<int, ExtrusionWrap>();
			var elements = new FilteredElementCollector(_doc).OfClass(typeof(Extrusion))
															 .OfType<Extrusion>();
			foreach (var elem in elements)
			{
				var extrusion = new ExtrusionWrap(elem);
				retl[extrusion.Id] = extrusion;
			}

			return retl;
		}
	}
}