using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;

namespace Logics.Export{

	public class ExtrusionWrap : AbstractGenericForm
	{
		public ExtrusionWrapParameters ExtrusionWrapProperties;
		public ExtrusionWrap(Extrusion ex) : base(ex)
		{
			ExtrusionWrapParameters _props = new ExtrusionWrapParameters();

			_props.StartOffset = ex.StartOffset;

			_props.EndOffset = ex.EndOffset;
			
			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", ex.Sketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", ex.Sketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.SketchPlane = sketchPlane;

			List<Dictionary<string, double[]>> curDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray curveArrArray = ex.Sketch.Profile;
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
														   cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
						curDicList.Add(curDic);
						lineNames += 1;
					}
					else if (cur.IsCyclic)
                    {
						Arc arc = cur as Arc;
						curDic = new Dictionary<string, double[]>()
														{ {"Array" + curveArrayNames + "Arc" + lineNames,
														   arc.GetEndPoint(0).ToJsonDoubles().
													Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
													Concat( GetPointOnArc(arc).ToJsonDoubles()).ToArray()} };
						curDicList.Add(curDic);
						lineNames += 1;
					}
					
                }
				curveArrayNames += 1;
				lineNames = 1;
            }

			_props.CurveArrArray = curDicList;

			_props.Id = ex.Id.IntegerValue;
			_props.IsSolid = ex.IsSolid;
			ExtrusionWrapProperties = _props;
		}

		private XYZ GetPointOnArc(Arc arc)
		{
			var pList = arc.Tessellate();
			return pList[1];
		}

		public ExtrusionWrap() {

		}

	}
	public class ExtrusionWrapParameters : AbstractGenericForm
	{
		public double StartOffset { get; set; }
		public double EndOffset { get; set; }
		public Dictionary<string, double[]>[] SketchPlane { get; set; }
		public List<Dictionary<string, double[]>> CurveArrArray { get; set; } //Profile
		private new int Id { get; set; }
		private new bool IsSolid { get; set; }

	}
}