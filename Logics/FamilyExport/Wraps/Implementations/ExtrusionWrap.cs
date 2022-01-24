using Autodesk.Revit.DB;
using System;
using Logics.FamilyExport.Wraps.Interfaces;

namespace Logics.FamilyExport{


	public class ExtrusionWrap : AbstractGenericForm
	{
		public double StartOffset { get; set; }
		public double EndOffset { get; set; }
		public double LengthOfExtrusion { get; set; }
        public int AmountOfSketchLines { get; set; }

        public ExtrusionWrap(Extrusion ex) : base(ex) {
			StartOffset = ex.StartOffset;
			EndOffset = ex.EndOffset;
			if (MathComparisonUtils.IsGreaterThan(Math.Abs(ex.StartOffset), Math.Abs(ex.EndOffset)))
            {
				LengthOfExtrusion = ex.StartOffset - ex.EndOffset;
            }
			else
            {
				LengthOfExtrusion = ex.EndOffset - ex.StartOffset;
            }

			int sumLines = 0;
			CurveArrArray curveArrArray = ex.Sketch.Profile;
			foreach (CurveArray i in curveArrArray)
            {
				foreach (Curve line in i)
                {
					sumLines += 1;
                }
            }
			AmountOfSketchLines = sumLines;
		}

		public ExtrusionWrap() {

		}

		
	}
}