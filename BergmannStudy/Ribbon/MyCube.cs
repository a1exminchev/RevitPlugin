using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Functions;

namespace BergmannStudy.Ribbon
{
    public class MyCube
    {
        public static void MyCubeExecute(Document doc)
        {
            SketchPlane sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero));
            Extrusion rectExtrusion = MyExtrusions.CreateCubeExtrusion(doc, sketchPlane);

			ReferenceArray raXInside = new ReferenceArray();
			ReferenceArray raYInside = new ReferenceArray();
			ReferenceArray sketchXLines = new ReferenceArray();
			ReferenceArray sketchYLines = new ReferenceArray();

			ReferenceArray raX = new ReferenceArray();
			ReferenceArray raY = new ReferenceArray();
			Options op = new Options();
			op.ComputeReferences = true;
			op.IncludeNonVisibleObjects = true;
			GeometryElement geomElem = rectExtrusion.get_Geometry(op);
			IEnumerator<GeometryObject> Objects = geomElem.GetEnumerator();
			FaceArray faces = null;
			while (Objects.MoveNext())
			{
				object o = Objects.Current;
				Solid solid = o as Solid;
				if (null == solid)
				{
					continue;
				}
				faces = solid.Faces;
			}
			foreach (Face face in faces)
			{
				PlanarFace pf = face as PlanarFace;
				if (null != pf && GeometryOperations.isParallel(XYZ.BasisX, pf.FaceNormal) == true)
				{
					raX.Append(pf.Reference);
				}
				else if (null != pf && GeometryOperations.isParallel(XYZ.BasisY, pf.FaceNormal) == true)
				{
					raY.Append(pf.Reference);
				}
			}

			XYZ p1 = new XYZ(500 / 304.8, 0, 0);
			XYZ p2 = new XYZ(500 / 304.8, 500 / 304.8, 0);
			XYZ p3 = new XYZ(0, 500 / 304.8, 0);
			Line line1 = Line.CreateBound(p1, p2);
			Line line2 = Line.CreateBound(p2, p3);

			Dimension dimXCub = doc.FamilyCreate.NewDimension(doc.ActiveView, line2, raX);
			XYZ transPoint2 = new XYZ(0, 250 / 304.8, 0);
			ElementTransformUtils.MoveElement(doc, dimXCub.Id, transPoint2);
			Dimension dimYCub = doc.FamilyCreate.NewDimension(doc.ActiveView, line1, raY);
			XYZ transPoint3 = new XYZ(250 / 304.8, 0, 0);
			ElementTransformUtils.MoveElement(doc, dimYCub.Id, transPoint3);

			var filterRefPlanes = new FilteredElementCollector(doc).OfClass(typeof(ReferencePlane));
			List<Element> listRefPlanes = filterRefPlanes.ToList();
			foreach (Element refPlaneEl in listRefPlanes)
			{
				ReferencePlane refPlane = refPlaneEl as ReferencePlane;
				Line line = refPlane.GetGeometryObjectFromReference(refPlane.GetReference()) as Line;
				if (GeometryOperations.isParallel(XYZ.BasisX, refPlane.Normal) == true)
				{
					raX.Append(refPlane.GetReference());
					raYInside.Append(new Reference(doc.GetElement(refPlane.GetReference())));
				}
				else if (GeometryOperations.isParallel(XYZ.BasisY, refPlane.Normal) == true)
				{
					raY.Append(refPlane.GetReference());
					raXInside.Append(new Reference(doc.GetElement(refPlane.GetReference())));
				}
			}

			Dimension dimXCom = doc.FamilyCreate.NewDimension(doc.ActiveView, line2, raX);
			Dimension dimYCom = doc.FamilyCreate.NewDimension(doc.ActiveView, line1, raY);

			dimXCom.AreSegmentsEqual = true;
			dimYCom.AreSegmentsEqual = true;

			CurveArrArray curveArrayProfile = rectExtrusion.Sketch.Profile;

			foreach (CurveArray curArr in curveArrayProfile)
			{
				foreach (Curve curve in curArr)
				{
					Line line = curve as Line;
					if (GeometryOperations.isParallel(XYZ.BasisX, line.Direction) == true)
					{
						sketchXLines.Append(new Reference(doc.GetElement(line.Reference)));
						raXInside.Append(new Reference(doc.GetElement(line.Reference)));
					}
					else if (GeometryOperations.isParallel(XYZ.BasisY, line.Direction) == true)
					{
						sketchYLines.Append(new Reference(doc.GetElement(line.Reference)));
						raYInside.Append(new Reference(doc.GetElement(line.Reference)));
					}
				}
			}
			Dimension dimXCubSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, line1, sketchXLines);
			ElementTransformUtils.MoveElement(doc, dimXCubSketch.Id, new XYZ(-1250 / 304.8, 0, 0));
			Dimension dimYCubSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, line2, sketchYLines);
			ElementTransformUtils.MoveElement(doc, dimYCubSketch.Id, new XYZ(0, -1250 / 304.8, 0));

			Dimension dimXComSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, line1, raXInside);
			ElementTransformUtils.MoveElement(doc, dimXComSketch.Id, new XYZ(-1000 / 304.8, 0, 0));
			Dimension dimYComSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, line2, raYInside);
			ElementTransformUtils.MoveElement(doc, dimYComSketch.Id, new XYZ(0, -1000 / 304.8, 0));
			dimXComSketch.AreSegmentsEqual = true;

			dimYComSketch.AreSegmentsEqual = true;

			FamilyManager famMan = doc.FamilyManager;
			FamilyParameter W = famMan.AddParameter("W", BuiltInParameterGroup.PG_GENERAL, ParameterType.Length, true);
			Parameter height = rectExtrusion.get_Parameter(BuiltInParameter.EXTRUSION_END_PARAM);
			dimXCubSketch.FamilyLabel = W;
			dimYCubSketch.FamilyLabel = W;
			dimXCub.FamilyLabel = W;
			dimYCub.FamilyLabel = W;
			famMan.AssociateElementParameterToFamilyParameter(height, W);
		}
    }
}
