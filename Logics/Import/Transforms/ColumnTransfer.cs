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
    public class ColumnTransfer : AbstractTransfer
    {
		public ColumnTransfer() {
			
        }
		//Отвечает за создание элемента в модели по полученному экземпляру из Json
		public ColumnWrapParameters ColumnWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			XYZ location = new XYZ(ColumnWrapProperties.CenterPoint[0],
								   ColumnWrapProperties.CenterPoint[0],
								   ColumnWrapProperties.CenterPoint[0]);

			var lvlFilter = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType();
			Level baseLevel = lvlFilter.Where(x => x.Name == ColumnWrapProperties.BottomLevelName)?.First() as Level;
			Level topLevel = lvlFilter.Where(x => x.Name == ColumnWrapProperties.TopLevelName)?.First() as Level;

			var famSymFilter = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Columns).WhereElementIsElementType();
			FamilySymbol famSym = famSymFilter.Where(x => x.Name == ColumnWrapProperties.FamilySymbolName) as FamilySymbol;

			Element el = docToImport.Create.NewFamilyInstance(location, famSym, baseLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);

			el.SetIdEntityToElement(ColumnWrapProperties.Id);
		}
	}
}
