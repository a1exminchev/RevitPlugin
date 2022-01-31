using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.FamilyExport.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.FamilyExport.ModelExport;

namespace Logics.FamilyExport{

	public class ExtrusionWrap : AbstractGenericForm
	{
		public ExtrusionWrapParameters ExtrusionWrapProperties;
		public ExtrusionWrap(Extrusion ex) : base(ex)
		{
			ExtrusionWrapParameters _props = new ExtrusionWrapParameters();

			_props.isSolid = new Dictionary<string, bool>() {
				{ "IsSolid", ex.IsSolid }
			};

			_props.StartOffset = new Dictionary<string, double>() {
				{ "StartOffset", ex.StartOffset }
			};

			_props.EndOffset = new Dictionary<string, double>() {
				{ "EndOffset", ex.EndOffset }
			};
			
			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", ex.Sketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", ex.Sketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.SketchPlane = new Dictionary<string, Dictionary<string, double[]>[]>() { { "SketchPlane", sketchPlane } };

			List<Dictionary<string, double[]>> curDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray curveArrArray = ex.Sketch.Profile;
			int curveArrayNames = 1;
			int lineNames = 1;
			foreach (CurveArray curveArray in curveArrArray)
            {
				foreach (Curve cur in curveArray)
                {
					Dictionary<string, double[]> curDic = new Dictionary<string, double[]>()
														{ {"Array" + curveArrayNames + "Line" + lineNames,
														   cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
					curDicList.Add(curDic);
					lineNames += 1;
                }
				curveArrayNames += 1;
				lineNames = 1;
            }
			
			_props.CurveArrArray = new Dictionary<string, List<Dictionary<string, double[]>>>() { { "CurveArrArray", curDicList } };

			_props.Id = ex.Id.IntegerValue;
			ExtrusionWrapProperties = _props;
		}

		public ExtrusionWrap() {

		}
	}
	public class ExtrusionWrapParameters : AbstractGenericForm
	{
		public Dictionary<string, bool> isSolid { get; set; }
		public Dictionary<string, double> StartOffset { get; set; }
		public Dictionary<string, double> EndOffset { get; set; }
		public Dictionary<string, Dictionary<string, double[]>[]> SketchPlane { get; set; }
		public Dictionary<string, List<Dictionary<string, double[]>>> CurveArrArray { get; set; } //Profile
		private new int Id { get; set; }
	}
}