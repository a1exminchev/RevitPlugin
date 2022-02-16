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

	public class StructuralColumnWrap : AbstractElementData
	{
		public StructuralColumnWrapParameters StructuralColumnWrapProperties;
		public StructuralColumnWrap(Element el) : base(el)
		{
            StructuralColumnWrapParameters _props = new StructuralColumnWrapParameters();
            FamilyInstance fam = el as FamilyInstance;
            Document _doc = el.Document;

            _props.FamilySymbolName = fam.Symbol.Name;

            Level level = _doc.GetElement(fam.LevelId) as Level;
            _props.LevelName = level.Name;

            var locP = fam.Location as LocationPoint;
            _props.CenterPoint = locP.Point.ToJsonDoubles();

            _props.Id = el.Id.IntegerValue;
            StructuralColumnWrapProperties = _props;
        }

		public StructuralColumnWrap() {

		}

	}
	public class StructuralColumnWrapParameters : AbstractElementData
	{
		public double[] CenterPoint { get; set; }
		public string FamilySymbolName { get; set; }
		public string LevelName { get; set; }
		private new int Id { get; set; }

	}
}