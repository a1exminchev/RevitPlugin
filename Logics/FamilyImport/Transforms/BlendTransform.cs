using System;
using System.Collections.Generic;
using System.Linq;
using rev = Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Logics.Geometry.Implementation;
using Autodesk.Revit.DB;

namespace Logics.FamilyImport.Transforms
{
    public class BlendTransform : AbstractTransform
    {
		public BlendTransform() {
			
        }

		public override void Create(rev.Document docToImport)
		{
			BlendParameters blParams = new BlendParameters();

			blParams.isSolid = isSolid;

			blParams.TopOffset = TopOffset;

			blParams.BottomOffset = BottomOffset;

			rev.XYZ skOrigin = new rev.XYZ(BaseSketchPlane["SketchOrigin"][0],
										  BaseSketchPlane["SketchOrigin"][1],
										  BaseSketchPlane["SketchOrigin"][2]);
			rev.XYZ skNormal = new rev.XYZ(BaseSketchPlane["SketchNormal"][0],
										  BaseSketchPlane["SketchNormal"][1],
										  BaseSketchPlane["SketchNormal"][2]);
			blParams.BaseSketchPlane = rev.SketchPlane.Create(docToImport, rev.Plane.CreateByNormalAndOrigin(skNormal, skOrigin));

			var skPlane = blParams.BaseSketchPlane.GetPlane();
			var transform = Transform.CreateTranslation(skPlane.Normal);

			rev.CurveArray topCurveArray = new rev.CurveArray();
			int topNumLine = 1;
			foreach (var pair in TopCurveArrArray)
            {
				if (pair.Key.Trim('L', 'i', 'n', 'e') == topNumLine.ToString())
                {
					topCurveArray.Append(rev.Line.CreateBound(new rev.XYZ(TopCurveArrArray[$"Line{topNumLine}"][0],
																TopCurveArrArray[$"Line{topNumLine}"][1],
																TopCurveArrArray[$"Line{topNumLine}"][2]),
													new rev.XYZ(TopCurveArrArray[$"Line{topNumLine}"][3],
																TopCurveArrArray[$"Line{topNumLine}"][4],
																TopCurveArrArray[$"Line{topNumLine}"][5])).CreateTransformed(transform));
					topNumLine += 1;
				}
				else if (pair.Key.Trim('A', 'r', 'c') == topNumLine.ToString())
				{
					topCurveArray.Append(rev.Arc.Create(new rev.XYZ(TopCurveArrArray[$"Arc{topNumLine}"][0],
															TopCurveArrArray[$"Arc{topNumLine}"][1],
															TopCurveArrArray[$"Arc{topNumLine}"][2]),
												new rev.XYZ(TopCurveArrArray[$"Arc{topNumLine}"][3],
															TopCurveArrArray[$"Arc{topNumLine}"][4],
															TopCurveArrArray[$"Arc{topNumLine}"][5]),
												new rev.XYZ(TopCurveArrArray[$"Arc{topNumLine}"][6],
															TopCurveArrArray[$"Arc{topNumLine}"][7],
															TopCurveArrArray[$"Arc{topNumLine}"][8])).CreateTransformed(transform));
					topNumLine += 1;
				}
			}
			blParams.TopCurveArray = topCurveArray;

			rev.CurveArray botCurveArray = new rev.CurveArray();
			int botNumLine = 1;
			foreach (var pair in BaseCurveArrArray)
			{
				if (pair.Key.Split('e')[1] == botNumLine.ToString())
				{
					botCurveArray.Append(rev.Line.CreateBound(new rev.XYZ(BaseCurveArrArray[$"Line{botNumLine}"][0],
																BaseCurveArrArray[$"Line{botNumLine}"][1],
																BaseCurveArrArray[$"Line{botNumLine}"][2]),
													new rev.XYZ(BaseCurveArrArray[$"Line{botNumLine}"][3],
																BaseCurveArrArray[$"Line{botNumLine}"][4],
																BaseCurveArrArray[$"Line{botNumLine}"][5])));
					botNumLine += 1;
				}
				else if (pair.Key.Split('c')[1] == botNumLine.ToString())
				{
					botCurveArray.Append(rev.Arc.Create(new rev.XYZ(BaseCurveArrArray[$"Arc{botNumLine}"][0],
															BaseCurveArrArray[$"Arc{botNumLine}"][1],
															BaseCurveArrArray[$"Arc{botNumLine}"][2]),
												new rev.XYZ(BaseCurveArrArray[$"Arc{botNumLine}"][3],
															BaseCurveArrArray[$"Arc{botNumLine}"][4],
															BaseCurveArrArray[$"Arc{botNumLine}"][5]),
												new rev.XYZ(BaseCurveArrArray[$"Arc{botNumLine}"][6],
															BaseCurveArrArray[$"Arc{botNumLine}"][7],
															BaseCurveArrArray[$"Arc{botNumLine}"][8])));
					botNumLine += 1;
				}
			}
			blParams.BaseCurveArray = botCurveArray;

			BlendCreator blendCreator = new BlendCreator(docToImport, blParams);
			blendCreator.Create();
		}

		public bool isSolid { get; set; }
		public double TopOffset { get; set; }
		public double BottomOffset { get; set; }
		public Dictionary<string, List<double>> BaseSketchPlane { get; set; }
		public Dictionary<string, List<double>> TopCurveArrArray { get; set; }
		public Dictionary<string, List<double>> BaseCurveArrArray { get; set; }
		private int Id { get; set; }
	}
}
