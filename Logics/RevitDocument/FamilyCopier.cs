using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using CSharpFunctionalExtensions;
using Logics.Geometry.Implementation;
using Result = CSharpFunctionalExtensions.Result;

namespace Logics.RevitDocument{
	public class FamilyCopier{
		private readonly Application   _app;
		private readonly UIApplication _uiApp;
		private          Document      _newDoc;

		public FamilyCopier(Application app, UIApplication uiApp) {
			_app   = app;
			_uiApp = uiApp;
		}

		public Document CopyFamilyDoc(Document doc) {
			if (!doc.IsFamilyDocument) {
				return null;
			}

			_newDoc = CreateNewDoc(doc);
			if (_newDoc == null) {
				return _newDoc;
			}

			var genericForms = new FilteredElementCollector(doc)
			                   .OfClass(typeof(GenericForm))
			                   .Cast<GenericForm>()
			                   .ToList();
			Transaction t = new Transaction(_newDoc, "New Doc");
			using (t) {
				t.Start();
				foreach (var genericForm in genericForms) {
					CopyGenericFormToNewDoc(genericForm);
				}

				t.Commit();
			}

			return _newDoc;
		}

		private Result<GenericForm> CopyGenericFormToNewDoc(GenericForm genericForm) {
			var createdForm = genericForm switch{
				Extrusion ex => Result.Success(CreateExtrusionInNewDoc(ex))
				, Blend bl => CreateBlendInNewDoc(bl)
				, Sweep sw => CreateSweepInNewDoc(sw)
				, SweptBlend swB => CreateSweptBlendInNewDoc(swB)
				, Revolution rev => CreateRevolutionInNewDoc(rev)
				, _ => Result.Failure<GenericForm>($"Invalid type for copy {genericForm.Name}")
			};
			 
			return createdForm;
		}

		private GenericForm CreateExtrusionInNewDoc(Extrusion extrusion) {
			Extrusion newEx;
			ExtrusionParameters extrusionParameters = new ExtrusionParameters();
			extrusionParameters.isSolid = extrusion.IsSolid;
			extrusionParameters.curveArrArray = extrusion.Sketch.Profile;
			SketchPlane newSketchPlane = SketchPlane.Create(_newDoc, extrusion.Sketch.SketchPlane.GetPlane());
			extrusionParameters.SketchPlane = newSketchPlane;
			extrusionParameters.Height = extrusion.EndOffset;
			var geometryElementCreator = new ExtrusionCreator(_newDoc, extrusionParameters);
			newEx = geometryElementCreator.Create() as Extrusion;
			newEx.StartOffset = extrusion.StartOffset;
			return newEx;
		}

		private GenericForm CreateBlendInNewDoc(Blend blend) {
			BlendParameters blendParameters = new BlendParameters();
			blendParameters.isSolid = blend.IsSolid;

			SketchPlane newBottomSketchPlane = SketchPlane.Create(_newDoc, blend.BottomSketch.SketchPlane.GetPlane());
			blendParameters.BaseSketchPlane = newBottomSketchPlane;
			var skPlane   = newBottomSketchPlane.GetPlane();
			var transform = Transform.CreateTranslation(skPlane.Normal);

			blendParameters.BaseCurveArray = TakeSingleCurveArray(blend.BottomSketch.Profile);
			List<Curve> topCurveList = TakeSingleCurveArray(blend.TopSketch.Profile).Cast<Curve>().Select(x => x.CreateTransformed(transform)).ToList();
			CurveArray  topCurArr    = new CurveArray();
			foreach (Curve cur in topCurveList) {
				topCurArr.Append(cur);
			}

			blendParameters.TopCurveArray = topCurArr;

			blendParameters.TopOffset    = blend.TopOffset;
			blendParameters.BottomOffset = blend.BottomOffset;

			var geometryElementCreator = new BlendCreator(_newDoc, blendParameters);
			return geometryElementCreator.Create() as Blend;
		}

		private GenericForm CreateSweepInNewDoc(Sweep sweep) {
			//В заметках наработки по выравниванию для создания повернутых форм, удалять жалко
			SweepParameters sweepParameters = new SweepParameters();
			sweepParameters.isSolid = sweep.IsSolid;
			SketchPlane pathPlane0        = SketchPlane.Create(_newDoc, Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero)); //XZ plane
			SketchPlane swPathSketchPlane = SketchPlane.Create(_newDoc, sweep.PathSketch.SketchPlane.GetPlane());
			sweepParameters.PathSketchPlane = pathPlane0;

			//double angleFromXZtoY = swPathSketchPlane.GetPlane().XVec.AngleOnPlaneTo(
			//Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisY, XYZ.BasisZ).YVec,
			//XYZ.BasisX);
			//double angleFromYZtoX = swPathSketchPlane.GetPlane().XVec.AngleOnPlaneTo(
			//Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisX, XYZ.BasisZ).YVec,
			//XYZ.BasisY);
			//Line axisX = Line.CreateBound(XYZ.Zero, XYZ.BasisX);
			//Line axisY = Line.CreateBound(XYZ.Zero, XYZ.BasisY);
			//TaskDialog.Show("d", angleFromXZtoY * 180 / Math.PI + " " + angleFromYZtoX * 180 / Math.PI);

			CurveArray pathCurArr0   = new CurveArray();
			CurveArray oldPathCurArr = TakeSingleCurveArray(sweep.PathSketch.Profile);
			//ElementTransformUtils.RotateElement(_newDoc, newPathSketchPlane.Id, axisX, angleFromXZtoY);
			//ElementTransformUtils.RotateElement(_newDoc, newPathSketchPlane.Id, axisY, angleFromYZtoX);
			//Transform tX = Transform.CreateRotation(XYZ.BasisX, angleFromXZtoY);
			//Transform tY = Transform.CreateRotation(XYZ.BasisY, angleFromYZtoX);
			foreach (Curve cur in oldPathCurArr) {
				//Curve curT = cur.CreateTransformed(tX).CreateTransformed(tY);
				//ModelCurve mCurve = _newDoc.FamilyCreate.NewModelCurve(curT, newPathSketchPlane);
				//pathCurArr0.Append(mCurve.GeometryCurve);
				pathCurArr0.Append(cur);
			}

			sweepParameters.PathCurveArray = pathCurArr0;

			//SketchPlane profilePlane0 = SketchPlane.Create(_newDoc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero)); //XY plane
			//SketchPlane newSk = SketchPlane.Create(_newDoc, sweep.ProfileSketch.SketchPlane.GetPlane());
			CurveArrArray curveArrArray = new CurveArrArray();
			CurveArrArray oldCurArrArr  = sweep.ProfileSketch.Profile;
			Transform     tProfile      = Transform.CreateTranslation(new XYZ(0 - swPathSketchPlane.GetPlane().Origin.X, 0 - swPathSketchPlane.GetPlane().Origin.Y, DeltaModul(sweep.ProfileSketch.SketchPlane.GetPlane().Origin.Z, sweep.ProfileSketch.SketchPlane.GetPlane().Origin.Z)));
			foreach (CurveArray curArr in oldCurArrArr) {
				CurveArray newCurveArray = new CurveArray();
				foreach (Curve cur in curArr) {
					//ModelCurve mCurve = _newDoc.FamilyCreate.NewModelCurve(cur, newSk);
					//newCurveArray.Append(mCurve.GeometryCurve);
					Curve curT = cur.CreateTransformed(tProfile);
					newCurveArray.Append(curT);
				}

				curveArrArray.Append(newCurveArray);
			}

			sweepParameters.ProfileCurveArrArray = curveArrArray;

			//sweepParameters.AngleFromXZtoY = angleFromXZtoY;
			//sweepParameters.AngleFromYZtoX = angleFromYZtoX;
			sweepParameters.WhichPathLineIsForProfile = 0;
			var geometryElementCreator = new SweepCreator(_newDoc, sweepParameters);
			return geometryElementCreator.Create() as Sweep;
		}

		private GenericForm CreateSweptBlendInNewDoc(SweptBlend sweptblend) {
			SweptBlendParameters sweptBlendPar = new SweptBlendParameters();
			sweptBlendPar.isSolid = sweptblend.IsSolid;
			Curve newPathCurve = Line.CreateBound(TakeSingleCurveArray(sweptblend.PathSketch.Profile).get_Item(0).GetEndPoint(0), TakeSingleCurveArray(sweptblend.PathSketch.Profile).get_Item(0).GetEndPoint(1));
			sweptBlendPar.PathCurve = newPathCurve;

			SketchPlane newPathSketchPlane = SketchPlane.Create(_newDoc, sweptblend.PathSketch.SketchPlane.GetPlane());
			sweptBlendPar.PathSketchPlane = newPathSketchPlane;

			SketchPlane newBotSkPl = SketchPlane.Create(_newDoc, sweptblend.BottomSketch.SketchPlane.GetPlane());
			SketchPlane newTopSkPl = SketchPlane.Create(_newDoc, sweptblend.TopSketch.SketchPlane.GetPlane());

			CurveArray botCurveList = TakeSingleCurveArray(sweptblend.BottomSketch.Profile);
			CurveArray topCurveList = TakeSingleCurveArray(sweptblend.TopSketch.Profile);
			CurveArray botCurArr    = new CurveArray();
			CurveArray topCurArr    = new CurveArray();
			Transform  tTop         = Transform.CreateTranslation(new XYZ(0 - newPathSketchPlane.GetPlane().Origin.X, 0 - newPathSketchPlane.GetPlane().Origin.Y, DeltaModul(sweptblend.TopSketch.SketchPlane.GetPlane().Origin.Z, newTopSkPl.GetPlane().Origin.Z)));
			Transform  tBot         = Transform.CreateTranslation(new XYZ(0 - newPathSketchPlane.GetPlane().Origin.X, 0 - newPathSketchPlane.GetPlane().Origin.Y, DeltaModul(sweptblend.BottomSketch.SketchPlane.GetPlane().Origin.Z, newBotSkPl.GetPlane().Origin.Z)));
			foreach (Curve cur in botCurveList) {
				Curve curT = cur.CreateTransformed(tBot);
				botCurArr.Append(curT);
			}

			foreach (Curve cur in topCurveList) {
				Curve curT = cur.CreateTransformed(tTop);
				topCurArr.Append(curT);
			}

			sweptBlendPar.Profile1CurveArray = botCurArr;
			sweptBlendPar.Profile2CurveArray = topCurArr;

			var geometryElementCreator = new SweptBlendCreator(_newDoc, sweptBlendPar);
			return geometryElementCreator.Create() as SweptBlend;
		}

		private GenericForm CreateRevolutionInNewDoc(Revolution revolution) {
			RevolutionParameters revolutionParameters = new RevolutionParameters();
			revolutionParameters.isSolid = revolution.IsSolid;
			Line newLine = Line.CreateBound(revolution.Axis.GeometryCurve.GetEndPoint(0), revolution.Axis.GeometryCurve.GetEndPoint(1));
			revolutionParameters.Axis = newLine;
			SketchPlane newSketchPlane = SketchPlane.Create(_newDoc, revolution.Sketch.SketchPlane.GetPlane());
			revolutionParameters.SketchPlane          = newSketchPlane;
			revolutionParameters.ProfileCurveArrArray = revolution.Sketch.Profile;
			revolutionParameters.StartingAngle        = revolution.StartAngle;
			revolutionParameters.EndingAngle          = revolution.EndAngle;
			var geometryElementCreator = new RevolutionCreator(_newDoc, revolutionParameters);
			return geometryElementCreator.Create() as Revolution;
		}

		private CurveArray TakeSingleCurveArray(CurveArrArray curveArrArray) {
			foreach (CurveArray curArr in curveArrArray) {
				return curArr;
			}

			return null;
		}

		private double DeltaModul(double elevation, double delta) {
			double _delta = 0;
			if (elevation > 0 && elevation - delta >= 0) {
				_delta = -delta;
				return _delta;
			}

			if (elevation < 0 && elevation + delta <= 0) {
				_delta = -delta;
				return _delta;
			}

			if (elevation > 0 && elevation - delta < 0) {
				_delta = elevation - delta;
				return _delta;
			}

			if (elevation < 0 && elevation + delta > 0) {
				_delta = -elevation + delta;
				return _delta;
			}

			return _delta;
		}

		private Document CreateNewDoc(Document doc) {
			FamilyCreator familyCreator = new FamilyCreator(_app);
			_newDoc = familyCreator.CreateNewFamily(_uiApp, "New Family Document", "Metric Generic Model");
			return _newDoc;
		}
	}
}