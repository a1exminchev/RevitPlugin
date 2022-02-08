using Logics.FamilyExport;
using Logics.FamilyImport.Transforms;
using System.Collections.Generic;
using System;

namespace Logics.FamilyImport
{
    public class FamilyDocumentData
    {
        public Dictionary<string, ExtrusionTransfer> Extrusions { get; set; }
        public Dictionary<string, RevolutionTransfer> Revolutions { get; set; }
        public Dictionary<string, BlendTransfer> Blends { get; set; }
        public Dictionary<string, SweepTransfer> Sweeps { get; set; }
        //public Dictionary<int, SweptBlendWrapParameters> SweptBlends { get; set; }

    }
}