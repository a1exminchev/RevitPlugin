using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.Export.ModelExport.Extractors.Implementations
{
    public class StructuralColumnExtracter : AbstractExtractor<StructuralColumnWrap>
    {
        public StructuralColumnExtracter(Document document) : base(document)
        {
        }

        public override Dictionary<int, StructuralColumnWrap> ExtractWork()
        {
            if (_doc.IsFamilyDocument == true)
            {
                return null;
            }
            var retl = new Dictionary<int, StructuralColumnWrap>();
            var elements = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements();
            foreach (var elem in elements)
            {
                var column = new StructuralColumnWrap(elem);
                retl[column.Id] = column;
            }

            return retl;
        }
    }
}