using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Export.Wraps.Implementations;

namespace Logics.Export{

	public class DimensionWrap : AbstractElementData
	{
		public DimensionWrapParameters DimensionWrapProperties;
		public DimensionWrap(Dimension dim) : base(dim)
		{
			DimensionWrapParameters _props = new DimensionWrapParameters();

			_props.ViewName = dim.View?.Name;

			Line line = dim.Curve as Line;
			Curve curve = Line.CreateBound(line.Origin, line.Direction);
			if (!curve.IsCyclic)
			{
				_props.LineAlongDim = new Dictionary<string, double[]>() { { "Line", curve.ToJsonDoubles() } };
			}
			else
			{
				_props.LineAlongDim = new Dictionary<string, double[]>() { { "Arc", curve.ToJsonDoubles() } };
			}

			ReferenceArray rfArr = dim.References;
			List<int[]> idList = new List<int[]>();
			Options opt = new Options();
			opt.ComputeReferences = true;
			opt.IncludeNonVisibleObjects = true;
			opt.DetailLevel = ViewDetailLevel.Fine;
			foreach(Reference rf in rfArr)
            {
				int idEl = rf.ElementId.IntegerValue;
				int[] elIdArray = { idEl };
                #region GenericForm
                GenericForm genForm = dim.Document.GetElement(rf.ElementId) as GenericForm;
				GeometryElement geoEl = genForm?.get_Geometry(opt);
				if (geoEl?.Count() != 0 && geoEl != null)
				{
					foreach (GeometryObject geoObj in geoEl)
					{
						Solid solid = geoObj as Solid;
						if (solid != null && solid.Faces.Size > 0)
						{
							foreach (Face face in solid.Faces)
							{
								if (Equals(rf.ConvertToStableRepresentation(dim.Document), face.Reference.ConvertToStableRepresentation(dim.Document)))
								{
									int[] elFaceIdArray = { idEl, face.Id };
									idList.Add(elFaceIdArray);
								}
							}
						}
						if (solid != null && solid.Edges.Size > 0)
						{
							foreach (Edge edge in solid.Edges)
							{
								if (Equals(rf.ConvertToStableRepresentation(dim.Document), edge.Reference.ConvertToStableRepresentation(dim.Document)))
								{
									int[] elEdgeIdArray = { idEl, edge.Id };
									idList.Add(elEdgeIdArray);
								}
								//if (Equals(rf.ConvertToStableRepresentation(dim.Document), 
								//	edge.AsCurve().GetEndPointReference(0).ConvertToStableRepresentation(dim.Document)))
        //                        {
								//	int[] elPointIdArray = { idEl, edge.Id, Convert.ToInt32(rf.ConvertToStableRepresentation(dim.Document).Split('/')[1]) };
								//	idList.Add(elPointIdArray);
								//}
							}
						}
						else
						{
							idList.Add(elIdArray);
						}
					}
				}
                #endregion
                #region ReferencePlane
                ReferencePlane refPlane = dim.Document.GetElement(rf.ElementId) as ReferencePlane;
                if (refPlane != null)
                {
                    int[] refPlaneIdArray = { refPlane.Id.IntegerValue };
                    idList.Add(refPlaneIdArray);
                }
                #endregion
                ModelCurve modelCurve = dim.Document.GetElement(rf.ElementId) as ModelCurve;
                Line geoLine = modelCurve?.GeometryCurve as Line;
                Arc geoArc = modelCurve?.GeometryCurve as Arc;
                Ellipse geoEllipse = modelCurve?.GeometryCurve as Ellipse;
                NurbSpline geoSpline = modelCurve?.GeometryCurve as NurbSpline;
                if (geoLine != null)
                {
                    int[] refModelCurveIdArray = { modelCurve.Id.IntegerValue };
                    idList.Add(refModelCurveIdArray);
                    continue;
                }
                else if (geoArc != null)
                {
                    if (geoArc.Reference.ConvertToStableRepresentation(dim.Document) == rf.ConvertToStableRepresentation(dim.Document))
                    {
                        int[] refModelCurveIdArray = { modelCurve.Id.IntegerValue };
                        idList.Add(refModelCurveIdArray);
                    }
                    else
                    {
                        int[] refModelCurveIdArray = { modelCurve.Id.IntegerValue, -1 }; // -1 means that should take centerpoint
                        idList.Add(refModelCurveIdArray);
                    }

                }

                _props.ReferenceIds = idList;
            }

			_props.Id = dim.Id.IntegerValue;
			DimensionWrapProperties = _props;
		}

		public DimensionWrap() {

		}

	}
	public class DimensionWrapParameters : AbstractElementData
	{
		public string ViewName { get; set; }
		public Dictionary<string, double[]> LineAlongDim { get; set; }
		public List<int[]> ReferenceIds { get; set; } //if there is 1 int - the reference in the object itself,
													  //if there is 2 int - 1st is the element, 2nd is the geoObject of element
		private new int Id { get; set; }

	}
}