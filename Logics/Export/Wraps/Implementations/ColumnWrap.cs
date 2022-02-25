using Autodesk.Revit.DB;
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

	public class ColumnWrap : AbstractElementData
	{
		public ColumnWrapParameters ColumnWrapProperties;
		public ColumnWrap(Element el) : base(el)
		{
            ColumnWrapParameters _props = new ColumnWrapParameters();
            FamilyInstance fam = el as FamilyInstance;
            Document _doc = el.Document;

            _props.FamilySymbolName = fam.Symbol.Name;

			var baseLvlId = fam.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsElementId();
			var topLvlId = fam.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsElementId();
			_props.BottomLevelName = _doc.GetElement(baseLvlId).Name;
			_props.TopLevelName = _doc.GetElement(topLvlId).Name;

			var locP = fam.Location as LocationPoint;
            _props.CenterPoint = locP.Point.ToJsonDoubles();

			_props.BottomOffset = fam.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();
			_props.TopOffset = fam.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();

			_props.Id = el.Id.IntegerValue;
            ColumnWrapProperties = _props;
        }

		public ColumnWrap() {

		}

	}
	public class ColumnWrapParameters : AbstractElementData
	{
		public double[] CenterPoint { get; set; }
		public string FamilySymbolName { get; set; }
		public string TopLevelName { get; set; }
		public string BottomLevelName { get; set; }
		public double BottomOffset { get; set; }
		public double TopOffset { get; set; }

	}
}