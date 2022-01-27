using Logics.FamilyExport;
using Logics.FamilyImport.Transforms;
using System.Collections.Generic;
using System;

namespace Logics.FamilyImport
{
    public class FamilyDocumentData
    {
        public List<ExtrusionTransform> Extrusions { get; set; }
        //public Dictionary<int, RevolutionWrapParameters> Revolutions { get; set; }
        //public Dictionary<int, BlendWrapParameters> Blends { get; set; }
        //public Dictionary<int, SweepWrapParameters> Sweeps { get; set; }
        //public Dictionary<int, SweptBlendWrapParameters> SweptBlends { get; set; }

    }
}