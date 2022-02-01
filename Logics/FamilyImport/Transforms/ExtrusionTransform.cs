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

			exParams.isSolid = isSolid;

			exParams.EndOffset = EndOffset;

			exParams.StartOffset = StartOffset;

			rev.XYZ skOrigin = new rev.XYZ(SketchPlane["SketchOrigin"][0],
										  SketchPlane["SketchOrigin"][1],
										  SketchPlane["SketchOrigin"][2]);
			rev.XYZ skNormal = new rev.XYZ(SketchPlane["SketchNormal"][0],
										  SketchPlane["SketchNormal"][1],
										  SketchPlane["SketchNormal"][2]);
			exParams.SketchPlane = rev.SketchPlane.Create(docToImport, rev.Plane.CreateByNormalAndOrigin(skNormal, skOrigin));


			rev.CurveArrArray curArrArr = new rev.CurveArrArray();
			rev.CurveArray curveArray = new rev.CurveArray();
			int numArr = 1;
			int numLine = 1;
			foreach (var pair in CurveArrArray)
            {
				if (pair.Key.Split('y')[1].Split('L')[0] == numArr.ToString())
                {
					if (pair.Key.Split('e')[1] == numLine.ToString())
                    {
						curveArray.Append(rev.Line.CreateBound(new rev.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][0],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][1],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][2]),
													   new rev.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][3],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][4],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][5])));
						numLine += 1;
					}
				}
				else if (pair.Key.Split('y')[1].Split('A')[0] == numArr.ToString())
                {
					if (pair.Key.Split('c')[1] == numLine.ToString())
					{
						curveArray.Append(rev.Arc.Create(new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][0],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][1],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][2]),
													new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][3],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][4],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][5]),
													new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][6],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][7],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][8])));

						numLine += 1;
					}
				}
                else
				{
					numArr += 1;
					numLine = 1;
					curArrArr.Append(curveArray);
					curveArray = new rev.CurveArray();
					if (pair.Key.Contains("Line"))
                    {
						curveArray.Append(rev.Line.CreateBound(new rev.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][0],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][1],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][2]),
													   new rev.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][3],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][4],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][5])));
					}
					else if (pair.Key.Contains("Arc"))
                    {
						curveArray.Append(rev.Arc.Create(new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][0],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][1],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][2]),
													new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][3],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][4],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][5]),
													new rev.XYZ(CurveArrArray[$"Array{numArr}Arc{numLine}"][6],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][7],
																CurveArrArray[$"Array{numArr}Arc{numLine}"][8])));
					}
					numLine = 2;
				}
			}
			curArrArr.Append(curveArray);
			exParams.curveArrArray = curArrArr;

			ExtrusionCreator extrusionCreator = new ExtrusionCreator(docToImport, exParams);
			extrusionCreator.Create();
		}

		public bool isSolid { get; set; }
		public double StartOffset { get; set; }
		public double EndOffset { get; set; }
		public Dictionary<string, List<double>> SketchPlane { get; set; }
		public Dictionary<string, List<double>> CurveArrArray { get; set; } //Profile
		private int Id { get; set; }
	}
}
