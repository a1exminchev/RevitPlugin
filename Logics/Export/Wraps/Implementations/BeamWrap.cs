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

	public class BeamWrap : AbstractElementData
	{
		public BeamWrapParameters BeamWrapProperties;
		public BeamWrap(Element el) : base(el)
		{
			BeamWrapParameters _props = new BeamWrapParameters();
			FamilyInstance fam = el as FamilyInstance;
			//Document _doc = el.Document;

			_props.FamilySymbolName = fam.Symbol.Name;

			Level level = doc.GetElement(fam.LevelId) as Level;
			_props.LevelName = level?.Name;

			Options opt = new Options();
			var curve = fam.get_Geometry(opt).Where(x => x as Curve != null)?.FirstOrDefault() as Curve;
			_props.AxisCurve = curve?.GetEndPoint(0).ToJsonDoubles().ToArray().Concat
							  (curve?.GetEndPoint(1).ToJsonDoubles()).ToArray();

			_props.Id = el.Id.IntegerValue;
			BeamWrapProperties = _props;
		}

		public BeamWrap() {

		}

	}
	public class BeamWrapParameters : AbstractElementData
	{
		public double[] AxisCurve { get; set; }
		public string FamilySymbolName { get; set; }
		public string LevelName { get; set; }
		private new int Id { get; set; }

	}
}