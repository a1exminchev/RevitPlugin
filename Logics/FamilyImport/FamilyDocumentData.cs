using Logics.FamilyExport;
using Logics.FamilyImport.Transforms;
using System.Collections.Generic;
using System;

namespace Logics.FamilyImport
{
    public class FamilyDocumentData
    {
        public Dictionary<string, ExtrusionTransform> Extrusions { get; set; }
        public Dictionary<string, RevolutionTransform> Revolutions { get; set; }
        public Dictionary<string, BlendTransform> Blends { get; set; }
        //public Dictionary<int, SweepWrapParameters> Sweeps { get; set; }
        //public Dictionary<int, SweptBlendWrapParameters> SweptBlends { get; set; }

    }
}