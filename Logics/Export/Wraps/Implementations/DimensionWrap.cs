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

				GenericForm el = dim.Document.GetElement(rf.ElementId) as GenericForm;
				GeometryElement geoEl = el.get_Geometry(opt);
				foreach (GeometryObject geoObj in geoEl)
                {
					Solid solid = geoObj as Solid;
					if (solid != null && solid.Faces.Size > 0)
                    {
						foreach (Face face in solid.Faces)
                        {
							if (Equals(rf.ConvertToStableRepresentation(dim.Document), face.Reference.ConvertToStableRepresentation(dim.Document)))
							{
								//ui.TaskDialog.Show("s", solid.Faces.Size.ToString()); +2
								int[] elFaceIdArray = { idEl, face.Id };
								idList.Add(elFaceIdArray);
							}
                        }
					}
					else
                    {
						idList.Add(elIdArray);
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