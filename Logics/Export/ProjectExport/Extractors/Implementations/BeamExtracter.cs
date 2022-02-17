using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.Export.ModelExport.Extractors.Implementations
{
    public class BeamExtracter : AbstractExtractor<BeamWrap>
    {
        public BeamExtracter(Document document) : base(document)
        {
        }

        public override Dictionary<int, BeamWrap> ExtractWork()
        {
            if (_doc.IsFamilyDocument == true)
            {
                return null;
            }
            var retl = new Dictionary<int, BeamWrap>();
            var elements = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements();
            foreach (var elem in elements)
            {
                var beam = new BeamWrap(elem);
                retl[beam.Id] = beam;
            }

            return retl;
        }
    }
}