using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;

namespace Logics.Export.ModelExport.Extractors.Implementations
{
    public class ColumnExtracter : AbstractExtractor<ColumnWrap>
    {
        public ColumnExtracter(Document document) : base(document)
        {
        }

        public override Dictionary<int, ColumnWrap> ExtractWork()
        {
            if (_doc.IsFamilyDocument == true)
            {
                return null;
            }
            var retl = new Dictionary<int, ColumnWrap>();
            var elements = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Columns).WhereElementIsNotElementType().ToElements();
            foreach (var elem in elements)
            {
                var column = new ColumnWrap(elem);
                retl[column.Id] = column;
            }

            return retl;
        }
    }
}