using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Geometry;

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
														{ { "Array" + curveArrayNames + "Line" + lineNames,
														   cur.ToJsonDoubles() } };
						curDicList.Add(curDic);
						lineNames += 1;
					}
					else if (cur.IsCyclic)
                    {
						curDic = new Dictionary<string, double[]>()
														{ { "Array" + curveArrayNames + "Arc" + lineNames,
														   cur.ToJsonDoubles() } };
						curDicList.Add(curDic);
						lineNames += 1;
					}
					
                }
				curveArrayNames += 1;
				lineNames = 1;
            }

			_props.CurveArrArray = curDicList;

			Options opt = new Options();
			opt.ComputeReferences = true;
			opt.DetailLevel = ViewDetailLevel.Fine;
			opt.IncludeNonVisibleObjects = true;
			List<Dictionary<int, double[]>> faceOriginsDict = new List<Dictionary<int, double[]>>();
			List<Dictionary<int, double[]>> edgeCurvesDict = new List<Dictionary<int, double[]>>();
			foreach (GeometryObject gOb in ex.get_Geometry(opt))
			{
				Solid solid = gOb as Solid;
				if (solid != null && solid.Faces.Size > 0)
				{
					foreach (Face face in solid.Faces)
					{
						PlanarFace pFace = face as PlanarFace;
						CylindricalFace cylFace = face as CylindricalFace;
						ConicalFace conFace = face as ConicalFace;
						RevolvedFace revFace = face as RevolvedFace;
						if (pFace != null)
                        {
							var faceDict = new Dictionary<int, double[]>() { { face.Id, pFace.Origin.ToJsonDoubles() } };
							faceOriginsDict.Add(faceDict);
						}
						if (cylFace != null)
						{
							var faceDict = new Dictionary<int, double[]>() { { face.Id, cylFace.Origin.ToJsonDoubles() } };
							faceOriginsDict.Add(faceDict);
						}
						if (conFace != null)
						{
							var faceDict = new Dictionary<int, double[]>() { { face.Id, conFace.Origin.ToJsonDoubles() } };
							faceOriginsDict.Add(faceDict);
						}
						if (revFace != null)
						{
							var faceDict = new Dictionary<int, double[]>() { { face.Id, revFace.Origin.ToJsonDoubles() } };
							faceOriginsDict.Add(faceDict);
						}
					}
				}
				if (solid != null && solid.Edges.Size > 0)
				{
					foreach (Edge edge in solid.Edges)
					{
						Curve curve = edge.AsCurve();
						Line line = curve as Line;
						Arc arc = curve as Arc;
						if (line != null && !curve.IsCyclic)
                        {
							var lineDict = new Dictionary<int, double[]>() { { edge.Id, line.Origin.ToJsonDoubles() } };
							edgeCurvesDict.Add(lineDict);
						}
						if (arc != null && curve.IsCyclic)
						{
							var arcDict = new Dictionary<int, double[]>() { { edge.Id, arc.Center.ToJsonDoubles().Concat(arc.Normal.ToJsonDoubles()).ToArray().Append(arc.Radius).ToArray() } };
							edgeCurvesDict.Add(arcDict);
						}
					}
				}
			}
			_props.FacesOrigins = faceOriginsDict;
			_props.EdgesCurves = edgeCurvesDict;

			_props.Id = ex.Id.IntegerValue;
			_props.IsSolid = ex.IsSolid;
			ExtrusionWrapProperties = _props;
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
		public List<Dictionary<int, double[]>> FacesOrigins { get; set; }
		public List<Dictionary<int, double[]>> EdgesCurves { get; set; }

	}
}