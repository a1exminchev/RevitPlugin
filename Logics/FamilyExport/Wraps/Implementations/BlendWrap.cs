using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.FamilyExport.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.FamilyExport.ModelExport;

namespace Logics.FamilyExport{


	public class BlendWrap : AbstractGenericForm
	{
		public BlendWrapParameters BlendWrapProperties;
		public BlendWrap(Blend bl) : base(bl) {
			BlendWrapParameters _props = new BlendWrapParameters();

			_props.isSolid = new Dictionary<string, bool>() {
				{ "IsSolid", bl.IsSolid }
			};

			_props.TopOffset = new Dictionary<string, double>() {
				{ "TopOffset", bl.TopOffset }
			};

			_props.BottomOffset = new Dictionary<string, double>() {
				{ "BottomOffset", bl.BottomOffset }
			};

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", bl.BottomSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", bl.BottomSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.BaseSketchPlane = new Dictionary<string, Dictionary<string, double[]>[]>() { { "BaseSketchPlane", sketchPlane } };

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
														{ {"Line" + topLineNames,
														   cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
						topCurDicList.Add(curDic);
						topLineNames += 1;
					}
					else if (cur.IsCyclic)
					{
						Arc arc = cur as Arc;
						curDic = new Dictionary<string, double[]>()
														{ {"Arc" + topLineNames,
														   arc.GetEndPoint(0).ToJsonDoubles().
													Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
													Concat( GetPointOnArc(arc).ToJsonDoubles()).ToArray()} };
						topCurDicList.Add(curDic);
						topLineNames += 1;
					}
				}
			}
			_props.TopCurveArrArray = new Dictionary<string, List<Dictionary<string, double[]>>>() { { "TopCurveArrArray", topCurDicList } };

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
														{ {"Line" + botLineNames,
														   cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
						botCurDicList.Add(curDic);
						botLineNames += 1;
					}
					else if (cur.IsCyclic)
					{
						Arc arc = cur as Arc;
						curDic = new Dictionary<string, double[]>()
														{ {"Arc" + botLineNames,
														   arc.GetEndPoint(0).ToJsonDoubles().
													Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
													Concat( GetPointOnArc(arc).ToJsonDoubles()).ToArray()} };
						botCurDicList.Add(curDic);
						botLineNames += 1;
					}
				}
			}
			_props.BaseCurveArrArray = new Dictionary<string, List<Dictionary<string, double[]>>>() { { "BaseCurveArrArray", botCurDicList } };

			_props.Id = bl.Id.IntegerValue;
			BlendWrapProperties = _props;
		}

		private XYZ GetPointOnArc(Arc arc)
		{
			var pList = arc.Tessellate();
			return pList[1];
		}

		public BlendWrap() {

		}
	}
	public class BlendWrapParameters : AbstractGenericForm
	{
		public Dictionary<string, bool> isSolid { get; set; }
		public Dictionary<string, double> TopOffset { get; set; }
		public Dictionary<string, double> BottomOffset { get; set; }
		public Dictionary<string, Dictionary<string, double[]>[]> BaseSketchPlane { get; set; }
		public Dictionary<string, List<Dictionary<string, double[]>>> TopCurveArrArray { get; set; } //1 Array
		public Dictionary<string, List<Dictionary<string, double[]>>> BaseCurveArrArray { get; set; } //1 Array
		private new int Id { get; set; }
	}
}