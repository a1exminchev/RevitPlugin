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
    public class BlendTransfer : AbstractTransfer
    {
		public BlendTransfer() {
			
        }
		//Отвечает за создание формы в модели по полученному экземпляру из Json
		public BlendWrapParameters BlendWrapProperties { get; set; }
		public override void Create(Document docToImport)
		{
			imp.BlendParameters blParams = new imp.BlendParameters();

			blParams.isSolid = BlendWrapProperties.IsSolid;

			blParams.TopOffset = BlendWrapProperties.TopOffset;

			blParams.BottomOffset = BlendWrapProperties.BottomOffset;

			XYZ skOrigin = new XYZ(BlendWrapProperties.BaseSketchPlane[0]["SketchOrigin"][0],
								   BlendWrapProperties.BaseSketchPlane[0]["SketchOrigin"][1],
								   BlendWrapProperties.BaseSketchPlane[0]["SketchOrigin"][2]);
			XYZ skNormal = new XYZ(BlendWrapProperties.BaseSketchPlane[1]["SketchNormal"][0],
								   BlendWrapProperties.BaseSketchPlane[1]["SketchNormal"][1],
								   BlendWrapProperties.BaseSketchPlane[1]["SketchNormal"][2]);
			blParams.BaseSketchPlane = SketchPlane.Create(docToImport, Plane.CreateByNormalAndOrigin(skNormal, skOrigin));

			var skPlane = blParams.BaseSketchPlane.GetPlane();
			var transform = Transform.CreateTranslation(skPlane.Normal);

			CurveArray topCurveArray = new CurveArray();
			int topNumLine = 1;
			foreach (var dict in BlendWrapProperties.TopCurveArrArray)
            {
				if (dict.FirstOrDefault().Key.Trim('L', 'i', 'n', 'e') == topNumLine.ToString())
                {
					topCurveArray.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
																  dict.FirstOrDefault().Value[1],
																  dict.FirstOrDefault().Value[2]),
														  new XYZ(dict.FirstOrDefault().Value[3],
																  dict.FirstOrDefault().Value[4],
																  dict.FirstOrDefault().Value[5])).CreateTransformed(transform));
					topNumLine += 1;
				}
				else if (dict.FirstOrDefault().Key.Trim('A', 'r', 'c') == topNumLine.ToString())
				{
					topCurveArray.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
															 dict.FirstOrDefault().Value[1],
															 dict.FirstOrDefault().Value[2]),
													 new XYZ(dict.FirstOrDefault().Value[3],
															 dict.FirstOrDefault().Value[4],
															 dict.FirstOrDefault().Value[5]),
													 new XYZ(dict.FirstOrDefault().Value[6],
															 dict.FirstOrDefault().Value[7],
															 dict.FirstOrDefault().Value[8])).CreateTransformed(transform));
					topNumLine += 1;
				}
			}
			blParams.TopCurveArray = topCurveArray;

			CurveArray botCurveArray = new CurveArray();
			int botNumLine = 1;
			foreach (var dict in BlendWrapProperties.BaseCurveArrArray)
			{
				if (dict.FirstOrDefault().Key.Split('e')[1] == botNumLine.ToString())
				{
					botCurveArray.Append(Line.CreateBound(new XYZ(dict.FirstOrDefault().Value[0],
																  dict.FirstOrDefault().Value[1],
																  dict.FirstOrDefault().Value[2]),
														  new XYZ(dict.FirstOrDefault().Value[3],
																  dict.FirstOrDefault().Value[4],
																  dict.FirstOrDefault().Value[5])));
					botNumLine += 1;
				}
				else if (dict.FirstOrDefault().Key.Split('c')[1] == botNumLine.ToString())
				{
					botCurveArray.Append(Arc.Create(new XYZ(dict.FirstOrDefault().Value[0],
															 dict.FirstOrDefault().Value[1],
															 dict.FirstOrDefault().Value[2]),
													 new XYZ(dict.FirstOrDefault().Value[3],
															 dict.FirstOrDefault().Value[4],
															 dict.FirstOrDefault().Value[5]),
													 new XYZ(dict.FirstOrDefault().Value[6],
															 dict.FirstOrDefault().Value[7],
															 dict.FirstOrDefault().Value[8])));
					botNumLine += 1;
				}
			}
			blParams.BaseCurveArray = botCurveArray;

			imp.BlendCreator blendCreator = new imp.BlendCreator(docToImport, blParams);
			Element el = blendCreator.Create();

			el.SetIdEntityToElement(BlendWrapProperties.Id);
		}
	}
}
