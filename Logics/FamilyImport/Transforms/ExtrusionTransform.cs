using System;
using System.Collections.Generic;
using System.Linq;
using rev = Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Logics.Geometry.Implementation;

namespace Logics.FamilyImport.Transforms
{
    public class ExtrusionTransform : AbstractTransform
    {
		public ExtrusionTransform() {
			
        }

		public override void Create(rev.Document docToImport)
		{
			ExtrusionParameters exParams = new ExtrusionParameters();
			exParams.isSolid = true; //Надо добавить это свойство!!!
			exParams.Height = EndOffset; //Надо переписать свойство Высота на Старт и Енд Оффсет

			rev.XYZ skOrigin = new rev.XYZ(SketchPlane["SketchOrigin"][0],
										  SketchPlane["SketchOrigin"][1],
										  SketchPlane["SketchOrigin"][2]);
			rev.XYZ skNormal = new rev.XYZ(SketchPlane["SketchNormal"][0],
										  SketchPlane["SketchNormal"][1],
										  SketchPlane["SketchNormal"][2]);
			exParams.SketchPlane = rev.SketchPlane.Create(docToImport, rev.Plane.CreateByNormalAndOrigin(skOrigin, skNormal));

			rev.CurveArrArray curArrArr = new rev.CurveArrArray();
			rev.CurveArray c1 = new rev.CurveArray();
			rev.CurveArray c2 = new rev.CurveArray();
			rev.CurveArray c3 = new rev.CurveArray();
			rev.CurveArray c4 = new rev.CurveArray();
			rev.CurveArray c5 = new rev.CurveArray();
			rev.CurveArray[] arrs = { c1, c2, c3, c4, c5};

			int numArr = 1;
			int numLine = 1;
			int arrays = 0;
			
			foreach (var pair in CurveArrArray)
            {
				if (pair.Key.Split('y')[1].Split('P')[0] == numArr.ToString())
                {
					arrays += 1;
					numArr += 1;
                }
				else
                {
					numArr = 1;
                }
            }
			for(int i = 0; i <= arrays; i++)
            {
				arrs.Append(new rev.CurveArray());
            }
			foreach (var pair in CurveArrArray)
            {
				if (pair.Key.Split('y')[1].Split('P')[0] == numArr.ToString())
                {
					if (pair.Key.Split('e')[1] == numLine.ToString())
                    {
						arrs[numArr - 1].Append(rev.Line.CreateBound(new rev.XYZ(CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][0],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][1],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][2]),
													   new rev.XYZ(CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][3],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][4],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][5])));
						numLine += 1;
					}
					else
                    {
						curArrArr.Append(arrs[numArr - 1]);
						numArr += 1;
					}
				}
				else
                {
					break;
                }
			}
			exParams.curveArrArray = curArrArr;
			TaskDialog.Show("d", curArrArr.get_Item(0).get_Item(1).GetEndPoint(0).ToString());
			ExtrusionCreator extrusionCreator = new ExtrusionCreator(docToImport, exParams);
			extrusionCreator.Create();
			//Creation of extrusion in doc by props
		}

		public double StartOffset { get; set; }
		public double EndOffset { get; set; }
		public Dictionary<string, List<double>> SketchPlane { get; set; }
		public Dictionary<string, List<double>> CurveArrArray { get; set; } //Profile
		//private int Id { get; set; }
	}
}
