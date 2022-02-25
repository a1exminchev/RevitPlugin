using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using imp = Logics.Geometry.Implementation;
using Logics.Export;

namespace Logics.Import.Transforms
{
    public class DimensionTransfer : AbstractTransfer
    {
		public DimensionTransfer() {
			
        }
		//Отвечает за создание размера в модели по полученному экземпляру из Json
		public DimensionWrapParameters DimensionWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			string viewName = DimensionWrapProperties.ViewName;
			var cl = new FilteredElementCollector(docToImport).OfCategory(BuiltInCategory.OST_Views).WhereElementIsNotElementType();
			View view = cl.Where(x => x.Name == viewName).FirstOrDefault() as View;

			var dict = DimensionWrapProperties.LineAlongDim;
			Line line = Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
												 dict.FirstOrDefault().Value[1],
												 dict.FirstOrDefault().Value[2]),
										 new XYZ(dict.FirstOrDefault().Value[3],
												 dict.FirstOrDefault().Value[4],
												 dict.FirstOrDefault().Value[5]));

			ReferenceArray refArr = new ReferenceArray();
			
			
		}
	}
}
