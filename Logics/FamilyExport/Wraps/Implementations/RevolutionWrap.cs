using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.FamilyExport.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.FamilyExport.ModelExport;

namespace Logics.FamilyExport{

	public class RevolutionWrap:AbstractGenericForm
	{
		public RevolutionWrapParameters RevolutionWrapProperties;
		public RevolutionWrap(Revolution rev) : base(rev) 
		{
			RevolutionWrapParameters _props = new RevolutionWrapParameters();

			_props.isSolid = new Dictionary<string, bool>() {
				{ "IsSolid", rev.IsSolid }
			};

			_props.StartingAngle = new Dictionary<string, double>() {
				{ "StartingAngle", rev.StartAngle }
			};

			_props.EndingAngle = new Dictionary<string, double>() {
				{ "EndingAngle", rev.EndAngle }
			};

			var axisDict = new Dictionary<string, double[]>() { { "PathLine", rev.Axis.GeometryCurve.GetEndPoint(0).ToJsonDoubles().
							  Concat(rev.Axis.GeometryCurve.GetEndPoint(1).ToJsonDoubles()).ToArray() } };
			Dictionary<string, double[]>[] pathDict = { axisDict };
			_props.PathLineDict = new Dictionary<string, Dictionary<string, double[]>[]>() { { "PathLineDict", pathDict } };

			var sketchOrigin = new Dictionary<string, double[]>() { { "SketchOrigin", rev.Sketch.SketchPlane.GetPlane().Origin.ToJsonDoubles() } };
			var sketchNormal = new Dictionary<string, double[]>() { { "SketchNormal", rev.Sketch.SketchPlane.GetPlane().Normal.ToJsonDoubles() } };
			Dictionary<string, double[]>[] sketchPlane = { sketchOrigin, sketchNormal };
			_props.SketchPlane = new Dictionary<string, Dictionary<string, double[]>[]>() { { "SketchPlane", sketchPlane } };

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

			_props.CurveArrArray = new Dictionary<string, List<Dictionary<string, double[]>>>() { { "CurveArrArray", curDicList } };

			_props.Id = rev.Id.IntegerValue;
			RevolutionWrapProperties = _props;

		}

		private XYZ GetPointOnArc(Arc arc)
		{
			var pList = arc.Tessellate();
			return pList[1];
		}

		public RevolutionWrap() {
		
		}
	}
	public class RevolutionWrapParameters : AbstractGenericForm
	{
		public Dictionary<string, bool> isSolid { get; set; }
		public Dictionary<string, double> StartingAngle { get; set; }
		public Dictionary<string, double> EndingAngle { get; set; }
		public Dictionary<string, Dictionary<string, double[]>[]> PathLineDict { get; set; }
		public Dictionary<string, Dictionary<string, double[]>[]> SketchPlane { get; set; }
		public Dictionary<string, List<Dictionary<string, double[]>>> CurveArrArray { get; set; } //Profile
		private new int Id { get; set; }
	}
}