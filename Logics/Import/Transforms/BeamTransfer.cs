using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using imp = Logics.Geometry.Implementation;
using Logics.Export;
using Logics.Import.ModelImport;

namespace Logics.Import.Transforms
{
    public class BeamTransfer : AbstractTransfer
    {
		public BeamTransfer() {
			
        }
		//Отвечает за создание элемента в модели по полученному экземпляру из Json
		public BeamWrapParameters BeamWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
            var dict = BeamWrapProperties.AxisCurve;
            Curve curve;
            if (dict.FirstOrDefault().Key == "Line")
            {
                curve = Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
                                                 dict.FirstOrDefault().Value[1],
                                                 dict.FirstOrDefault().Value[2]),
                                         new XYZ(dict.FirstOrDefault().Value[3],
                                                 dict.FirstOrDefault().Value[4],
                                                 dict.FirstOrDefault().Value[5]));
            }
            else if (dict.FirstOrDefault().Key == "Arc")
            {
                curve = Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
                                           dict.FirstOrDefault().Value[1],
                                           dict.FirstOrDefault().Value[2]),
                                   new XYZ(dict.FirstOrDefault().Value[3],
                                           dict.FirstOrDefault().Value[4],
                                           dict.FirstOrDefault().Value[5]),
                                   new XYZ(dict.FirstOrDefault().Value[6],
                                           dict.FirstOrDefault().Value[7],
                                           dict.FirstOrDefault().Value[8]));
            }
            else { curve = null; }

			var lvlFilter = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
			Level baseLevel = lvlFilter.Where(x => x.Name == BeamWrapProperties.LevelName)?.First() as Level;

			var famSymFilter = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Columns).WhereElementIsElementType();
			FamilySymbol famSym = famSymFilter.Where(x => x.Name == BeamWrapProperties.FamilySymbolName) as FamilySymbol;

			Element el = docToImport.Create.NewFamilyInstance(curve, famSym, baseLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);
            el.SetIdEntityToElement(BeamWrapProperties.Id);
        }
	}
}
