using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Geometry;

namespace Logics.Export{

	public class SweepWrap : AbstractGenericForm
	{
		public SweepWrapParameters SweepWrapProperties;
		public SweepWrap(Sweep sw) : base(sw)
		{
			SweepWrapParameters _props = new SweepWrapParameters();

			List<Dictionary<string, double[]>> pathCurDicList = new List<Dictionary<string, double[]>>();
			CurveArray curveArray = sw.PathSketch.Profile.get_Item(0);
			int lineNames = 1;
			foreach (Curve cur in curveArray)
			{
				Dictionary<string, double[]> curDic;
				if (!cur.IsCyclic)
				{
					curDic = new Dictionary<string, double[]>()
													{ {"Line" + lineNames, cur.ToJsonDoubles() } };
					pathCurDicList.Add(curDic);
					lineNames += 1;
				}
				else if (cur.IsCyclic)
				{
					curDic = new Dictionary<string, double[]>()
													{ {"Arc" + lineNames, cur.ToJsonDoubles() } };
					pathCurDicList.Add(curDic);
					lineNames += 1;
				}
			}
			_props.PathCurveArray = pathCurDicList;

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", sw.PathSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", sw.PathSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.PathSketchPlane = sketchPlane;

			List<Dictionary<string, double[]>> curDicList = new List<Dictionary<string, double[]>>();
			CurveArrArray curveArrArray = sw.ProfileSketch.Profile;
			int curveArrayNames = 1;
			lineNames = 1;
			foreach (CurveArray curArr in curveArrArray)
			{
				foreach (Curve cur in curArr)
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

			_props.ProfileCurveArrArray = curDicList;

			var prSketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", sw.ProfileSketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var prSketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", sw.ProfileSketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] profileSketchPlane = { prSketchOrigin, prSketchNormal };
			_props.ProfileSketchPlane = profileSketchPlane;

			_props.Id = sw.Id.IntegerValue;
			_props.IsSolid = sw.IsSolid;
			SweepWrapProperties = _props;

		}

		public SweepWrap()
		{

		}
	}
	public class SweepWrapParameters : AbstractGenericForm
	{
		public double AngleFromXZtoY { get; set; }
		public double AngleFromYZtoX { get; set; }
		public List<Dictionary<string, double[]>> PathCurveArray { get; set; }
		public Dictionary<string, double[]>[] PathSketchPlane { get; set; }
		public List<Dictionary<string, double[]>> ProfileCurveArrArray { get; set; }
		public Dictionary<string, double[]>[] ProfileSketchPlane { get; set; }
		public int WhichPathLineIsForProfile = 0;
		private new int Id { get; set; }
		private new bool IsSolid { get; set; }
	}
}