using Autodesk.Revit.DB;
using ui = Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using cr = Autodesk.Revit.Creation;
using System;
using System.Linq;
using System.Collections.Generic;
using Logics.Export.Wraps.Interfaces;
using Logics.Geometry.Implementation;
using Logics.Export.ModelExport;
using Logics.Export.Wraps.Implementations;

namespace Logics.Export{

	public class BeamWrap : AbstractElementData
	{
		public BeamWrapParameters BeamWrapProperties;
		public BeamWrap(Element el) : base(el)
		{
            BeamWrapParameters _props = new BeamWrapParameters();
            FamilyInstance fam = el as FamilyInstance;
            Document _doc = el.Document;
			
            _props.FamilySymbolName = fam.Symbol.Name;

			var LvlId = fam.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
			_props.LevelName = _doc.GetElement(LvlId).Name;

			var curLoc = fam.Location as LocationCurve;
			Curve curve = curLoc.Curve;
			if (!curve.IsCyclic)
            {
				_props.AxisCurve = new Dictionary<string, double[]>() { { "Line", curve.ToJsonDoubles() } };
			}
			else
            {
				_props.AxisCurve = new Dictionary<string, double[]>() { { "Arc", curve.ToJsonDoubles() } };
			}
			

			_props.StartOffset = fam.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
			_props.EndOffset = fam.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();

			_props.Id = el.Id.IntegerValue;
            BeamWrapProperties = _props;
        }

		public BeamWrap() {

		}

	}
	public class BeamWrapParameters : AbstractElementData
	{
		public Dictionary<string, double[]> AxisCurve { get; set; }
		public string FamilySymbolName { get; set; }
		public string LevelName { get; set; }
		public double StartOffset { get; set; }
		public double EndOffset { get; set; }
		private new int Id { get; set; }

	}
}