using System;
using System.Collections.Generic;
using System.Linq;
using revit = Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Logics.Geometry.Implementation;

namespace Logics.FamilyImport.Transforms
{
    public class RevolutionTransform : AbstractTransform
    {
		public RevolutionTransform() {
			
        }

		public override void Create(revit.Document docToImport)
		{
			RevolutionParameters revParams = new RevolutionParameters();

			revParams.isSolid = isSolid;

			revParams.EndingAngle = EndingAngle;

			revParams.StartingAngle = StartingAngle;

			revit.Line axisLine = revit.Line.CreateBound(new revit.XYZ(PathLineDict["PathLine"][0],
																	   PathLineDict["PathLine"][1],
																	   PathLineDict["PathLine"][2]),
														 new revit.XYZ(PathLineDict["PathLine"][3],
																	   PathLineDict["PathLine"][4],
																	   PathLineDict["PathLine"][5]));
			revParams.Axis = axisLine;

			revit.XYZ skOrigin = new revit.XYZ(SketchPlane["SketchOrigin"][0],
											   SketchPlane["SketchOrigin"][1],
											   SketchPlane["SketchOrigin"][2]);
			revit.XYZ skNormal = new revit.XYZ(SketchPlane["SketchNormal"][0],
											   SketchPlane["SketchNormal"][1],
											   SketchPlane["SketchNormal"][2]);
			revParams.SketchPlane = revit.SketchPlane.Create(docToImport, revit.Plane.CreateByNormalAndOrigin(skNormal, skOrigin));


			revit.CurveArrArray curArrArr = new revit.CurveArrArray();
			revit.CurveArray curveArray = new revit.CurveArray();
			int numArr = 1;
			int numLine = 1;
			foreach (var pair in CurveArrArray)
            {
				if (pair.Key.Split('y')[1].Split('L')[0] == numArr.ToString())
                {
					if (pair.Key.Split('e')[1] == numLine.ToString())
                    {
						curveArray.Append(revit.Line.CreateBound(new revit.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][0],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][1],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][2]),
													   new revit.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][3],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][4],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][5])));
						numLine += 1;
					}
				}
                else
				{
					numArr += 1;
					numLine = 1;
					curArrArr.Append(curveArray);
					curveArray = new revit.CurveArray();
					curveArray.Append(revit.Line.CreateBound(new revit.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][0],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][1],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][2]),
													   new revit.XYZ(CurveArrArray[$"Array{numArr}Line{numLine}"][3],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][4],
																   CurveArrArray[$"Array{numArr}Line{numLine}"][5])));
					numLine = 2;
				}
			}
			curArrArr.Append(curveArray);
			revParams.ProfileCurveArrArray = curArrArr;

			RevolutionCreator revolutionCreator = new RevolutionCreator(docToImport, revParams);
			revolutionCreator.Create();
		}

		public bool isSolid { get; set; }
		public double StartingAngle { get; set; }
		public double EndingAngle { get; set; }
		public Dictionary<string, List<double>> PathLineDict { get; set; }
		public Dictionary<string, List<double>> SketchPlane { get; set; }
		public Dictionary<string, List<double>> CurveArrArray { get; set; } //Profile
		private int Id { get; set; }
	}
}
