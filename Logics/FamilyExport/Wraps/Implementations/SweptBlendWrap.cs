using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.FamilyExport.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.FamilyExport.ModelExport;

namespace Logics.FamilyExport{


	public class SweptBlendWrap : AbstractGenericForm
	{
		public SweptBlendWrapParameters SweptBlendWrapProperties;
		public SweptBlendWrap(SweptBlend sw) : base(sw)
		{
			SweptBlendWrapParameters _props = new SweptBlendWrapParameters();

			Curve curvePath = sw.PathSketch.Profile.get_Item(0).get_Item(0);
			Dictionary<string, double[]> pathCurveDic = new Dictionary<string, double[]>();
			if (!curvePath.IsCyclic)
            {
				pathCurveDic = new Dictionary<string, double[]>()
								{ { "Line", curvePath.GetEndPoint(0).ToJsonDoubles().
									 Concat(curvePath.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
			}
			else if (curvePath.IsCyclic)
            {
				Arc arc = curvePath as Arc;
				pathCurveDic = new Dictionary<string, double[]>()
								{ { "Line", arc.GetEndPoint(0).ToJsonDoubles().
									 Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
									 Concat(GetPointOnArc(arc).ToJsonDoubles()).ToArray() } };
			}
			_props.PathCurve = pathCurveDic;

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", sw.PathSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", sw.PathSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.PathSketchPlane = sketchPlane;

			List<Dictionary<string, double[]>> curDicList1 = new List<Dictionary<string, double[]>>();
			CurveArray curveArray1 = sw.BottomProfile.get_Item(0);
			int lineNames = 1;
			foreach (Curve cur in curveArray1)
			{
				Dictionary<string, double[]> curDic;
				if (!cur.IsCyclic)
				{
					curDic = new Dictionary<string, double[]>()
													{ {"Line" + lineNames,
														cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
					curDicList1.Add(curDic);
					lineNames += 1;
				}
				else if (cur.IsCyclic)
				{
					Arc arc = cur as Arc;
					curDic = new Dictionary<string, double[]>()
													{ {"Arc" + lineNames,
														arc.GetEndPoint(0).ToJsonDoubles().
												Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
												Concat( GetPointOnArc(arc).ToJsonDoubles()).ToArray()} };
					curDicList1.Add(curDic);
					lineNames += 1;
				}

			}
			_props.Profile1CurveArray = curDicList1;

			List<Dictionary<string, double[]>> curDicList2 = new List<Dictionary<string, double[]>>();
			CurveArray curveArray2 = sw.TopProfile.get_Item(0);
			lineNames = 1;
			foreach (Curve cur in curveArray2)
			{
				Dictionary<string, double[]> curDic;
				if (!cur.IsCyclic)
				{
					curDic = new Dictionary<string, double[]>()
													{ {"Line" + lineNames,
														cur.GetEndPoint(0).ToJsonDoubles().Concat(cur.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
					curDicList2.Add(curDic);
					lineNames += 1;
				}
				else if (cur.IsCyclic)
				{
					Arc arc = cur as Arc;
					curDic = new Dictionary<string, double[]>()
													{ {"Arc" + lineNames,
														arc.GetEndPoint(0).ToJsonDoubles().
												Concat(arc.GetEndPoint(1).ToJsonDoubles()).ToArray().
												Concat( GetPointOnArc(arc).ToJsonDoubles()).ToArray()} };
					curDicList2.Add(curDic);
					lineNames += 1;
				}

			}
			_props.Profile2CurveArray = curDicList2;

			var pr1SketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", sw.BottomSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var pr1SketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", sw.BottomSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] profile1SketchPlane = { pr1SketchOrigin, pr1SketchNormal };
			_props.Profile1SketchPlane = profile1SketchPlane;

			var pr2SketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", sw.TopSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var pr2SketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", sw.TopSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] profile2SketchPlane = { pr2SketchOrigin, pr2SketchNormal };
			_props.Profile2SketchPlane = profile2SketchPlane;

			_props.Id = sw.Id.IntegerValue;
			_props.IsSolid = sw.IsSolid;
			SweptBlendWrapProperties = _props;

		}

		private XYZ GetPointOnArc(Arc arc)
		{
			var pList = arc.Tessellate();
			return pList[1];
		}

		public SweptBlendWrap()
		{

		}
	}
	public class SweptBlendWrapParameters : AbstractGenericForm
	{
		public Dictionary<string, double[]> PathCurve { get; set; }
		public Dictionary<string, double[]>[] PathSketchPlane { get; set; }
		public List<Dictionary<string, double[]>> Profile1CurveArray { get; set; }
		public List<Dictionary<string, double[]>> Profile2CurveArray { get; set; }
		public Dictionary<string, double[]>[] Profile1SketchPlane { get; set; }
		public Dictionary<string, double[]>[] Profile2SketchPlane { get; set; }
		private new int Id { get; set; }
		private new bool IsSolid { get; set; }
	}
}