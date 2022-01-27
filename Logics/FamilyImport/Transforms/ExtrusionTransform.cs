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
			int numArr = 1;
			int numLine = 1;
			foreach (var pair in CurveArrArray)
            {
				if (pair.Key.Split('y')[1].Split('P')[0] == numArr.ToString())
                {
					rev.CurveArray curArr = new rev.CurveArray();
					if (pair.Key.Split('e')[1] == numLine.ToString())
                    {
						curArr.Append(rev.Line.CreateBound(new rev.XYZ(CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][0],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][1],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][2]),
													   new rev.XYZ(CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][3],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][4],
																   CurveArrArray[$"Array{numArr}PointOfLine{numLine}"][5])));
						numLine += 1;
						
						goto P1;
					}
					else
                    {
						curArrArr.Append(curArr);
					}
					numArr += 1;
					
				}
				else
                {
					break;
                }
			P1:
				;
			}
			exParams.curveArrArray = curArrArr;
			//TaskDialog.Show("d", curArrArr.get_Item(0).get_Item(0).GetEndPoint(0).ToString());
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
