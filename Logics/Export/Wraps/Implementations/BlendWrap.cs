using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Geometry;

namespace Logics.Export{


	public class BlendWrap : AbstractGenericForm
	{
		public BlendWrapParameters BlendWrapProperties;
		public BlendWrap(Blend bl) : base(bl) {
			BlendWrapParameters _props = new BlendWrapParameters();

			_props.TopOffset = bl.TopOffset;

			_props.BottomOffset = bl.BottomOffset;

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", bl.BottomSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", bl.BottomSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.BaseSketchPlane = sketchPlane;

			List<Dictionary<string, double[]>> topCurDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray topCurveArrArray = bl.TopProfile;
			int topLineNames = 1;
			foreach (CurveArray curveArray in topCurveArrArray)
			{
				foreach (Curve cur in curveArray)
				{
					Dictionary<string, double[]> curDic;
					if (!cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ { "Line" + topLineNames, cur.ToJsonDoubles() } };
						topCurDicList.Add(curDic);
						topLineNames += 1;
					}
					else if (cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ { "Arc" + topLineNames, cur.ToJsonDoubles() } };
						topCurDicList.Add(curDic);
						topLineNames += 1;
					}
				}
			}
			_props.TopCurveArrArray = topCurDicList;

			List<Dictionary<string, double[]>> botCurDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray boCurveArrArray = bl.BottomProfile;
			int botLineNames = 1;
			foreach (CurveArray curveArray in boCurveArrArray)
			{
				foreach (Curve cur in curveArray)
				{
					Dictionary<string, double[]> curDic;
					if (!cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ { "Line" + botLineNames, cur.ToJsonDoubles() } };
						botCurDicList.Add(curDic);
						botLineNames += 1;
					}
					else if (cur.IsCyclic)
					{
						curDic = new Dictionary<string, double[]>()
														{ { "Arc" + botLineNames, cur.ToJsonDoubles() } };
						botCurDicList.Add(curDic);
						botLineNames += 1;
					}
				}
			}
			_props.BaseCurveArrArray = botCurDicList;

			_props.Id = bl.Id.IntegerValue;
			_props.IsSolid = bl.IsSolid;
			BlendWrapProperties = _props;
		}

		public BlendWrap() {

		}
	}
	public class BlendWrapParameters : AbstractGenericForm
	{
		public double TopOffset { get; set; }
		public double BottomOffset { get; set; }
		public Dictionary<string, double[]>[] BaseSketchPlane { get; set; }
		public List<Dictionary<string, double[]>> TopCurveArrArray { get; set; } //1 Array
		public List<Dictionary<string, double[]>> BaseCurveArrArray { get; set; } //1 Array

	}
}