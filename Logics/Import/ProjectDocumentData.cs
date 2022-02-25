using Logics.Export;
using Logics.Import.Transforms;
using System.Collections.Generic;
using System;

namespace Logics.Import
{
    public class ProjectDocumentData
    {
        public Dictionary<string, ColumnTransfer> Columns { get; set; }
        //public Dictionary<string, StructuralColumnTransfer> StructuralColumns { get; set; }
        //public Dictionary<string, BeamTransfer> Beams { get; set; }
        public Dictionary<string, DimensionTransfer> Dimensions { get; set; }

    }
}