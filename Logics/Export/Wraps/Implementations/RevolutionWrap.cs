using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Geometry;

namespace Logics.Export{

	public class RevolutionWrap:AbstractGenericForm
	{
		public RevolutionWrapParameters RevolutionWrapProperties;
		public RevolutionWrap(Revolution rev) : base(rev) 
		{
			RevolutionWrapParameters _props = new RevolutionWrapParameters();

			_props.StartingAngle = rev.StartAngle;

			_props.EndingAngle = rev.EndAngle;

			var axisDict = new Dictionary<string, double[]>() { { "PathLine", rev.Axis.GeometryCurve.GetEndPoint(0).ToJsonDoubles().
							  Concat(rev.Axis.GeometryCurve.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
			_props.PathLineDict = axisDict;

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", rev.Sketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", rev.Sketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.SketchPlane = sketchPlane;

			List<Dictionary<string, double[]>> curDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray curveArrArray = rev.Sketch.Profile;
			int curveArrayNames = 1;
			int lineNames = 1;
			foreach (CurveArray curveArray in curveArrArray)
			{
				foreach (Curve cur in curveArray)
				{
					Dictionary<string, double[]> curDic;
					if (!cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ {"Array" + curveArrayNames + "Line" + lineNames,
														   cur.ToJsonDoubles() } };
						curDicList.Add(curDic);
						lineNames += 1;
					}
					else if (cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ {"Array" + curveArrayNames + "Arc" + lineNames,
														   cur.ToJsonDoubles() } };
						curDicList.Add(curDic);
						lineNames += 1;
					}

				}
				curveArrayNames += 1;
				lineNames = 1;
			}

			_props.CurveArrArray = curDicList;

			_props.Id = rev.Id.IntegerValue;
			_props.IsSolid = rev.IsSolid;
			RevolutionWrapProperties = _props;

		}

		public RevolutionWrap() {
		
		}
	}
	public class RevolutionWrapParameters : AbstractGenericForm
	{
		public double StartingAngle { get; set; }
		public double EndingAngle { get; set; }
		public Dictionary<string, double[]> PathLineDict { get; set; }
		public Dictionary<string, double[]>[] SketchPlane { get; set; }
		public List<Dictionary<string, double[]>> CurveArrArray { get; set; } //Profile

	}
}