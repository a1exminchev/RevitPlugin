using System;
using System.Collections.Generic;
using System.Linq;
using Logics.Geometry;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;
using Logics.RevitDocument;

namespace StudyTask.Ribbon
{
    public class MyCube
    {
        private readonly Application _app;
        private Document _newDoc;
        public static void MyCubeExecute(Document doc)
        {
            SketchPlane sketchPlane = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero));
            CubeExtrusionParameters cubeParameters = new CubeExtrusionParameters();
            cubeParameters.Height = 500 / 304.8;
            cubeParameters.Width = 500 / 304.8;
            cubeParameters.CenterPoint = new XYZ(0, 0, 0);
            cubeParameters.SketchPlane = sketchPlane;
            cubeParameters.isSolid = true;
            IGenericFormCreator<CubeExtrusionParameters> geometryElementCreator = new CubeExtrusionCreator(doc, cubeParameters);
            Extrusion cubeExtrusion = geometryElementCreator.Create() as Extrusion;
            ReferenceArray raXInside = new ReferenceArray();
            ReferenceArray raYInside = new ReferenceArray();
            ReferenceArray sketchXLines = new ReferenceArray();
            ReferenceArray sketchYLines = new ReferenceArray();
            ReferenceArray raX = new ReferenceArray();
            ReferenceArray raY = new ReferenceArray();
            Options op = new Options();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;
            GeometryElement geomElem = cubeExtrusion.get_Geometry(op);
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
            XYZ p0 = new XYZ(0, 0, 0);
            XYZ p1 = new XYZ(1 / 304.8, 0, 0);
            XYZ p2 = new XYZ(0, 1 / 304.8, 0);
            Line lineX = Line.CreateBound(p0, p1);
            Line lineY = Line.CreateBound(p0, p2);

            double deltaY = (cubeExtrusion.get_BoundingBox(doc.ActiveView).Min.Y + cubeExtrusion.get_BoundingBox(doc.ActiveView).Max.Y) / 2;
            double deltaX = (cubeExtrusion.get_BoundingBox(doc.ActiveView).Min.X + cubeExtrusion.get_BoundingBox(doc.ActiveView).Max.X) / 2;
            Dimension dimXCub = doc.FamilyCreate.NewDimension(doc.ActiveView, lineX, raX);
            ElementTransformUtils.MoveElement(doc, dimXCub.Id, new XYZ(0, deltaY + 500 / 304.8, 0));
            Dimension dimYCub = doc.FamilyCreate.NewDimension(doc.ActiveView, lineY, raY);
            ElementTransformUtils.MoveElement(doc, dimYCub.Id, new XYZ(deltaX + 500 / 304.8, 0, 0));

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
            Dimension dimXCom = doc.FamilyCreate.NewDimension(doc.ActiveView, lineX, raX);
            ElementTransformUtils.MoveElement(doc, dimXCom.Id, new XYZ(0, deltaY + 700 / 304.8, 0));
            Dimension dimYCom = doc.FamilyCreate.NewDimension(doc.ActiveView, lineY, raY);
            ElementTransformUtils.MoveElement(doc, dimYCom.Id, new XYZ(deltaX + 700 / 304.8, 0, 0));

            CurveArrArray curveArrayProfile = cubeExtrusion.Sketch.Profile;
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
            Dimension dimXCubSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, lineX, sketchXLines);
            ElementTransformUtils.MoveElement(doc, dimXCubSketch.Id, new XYZ(0, deltaY + 500 / 304.8, 0));
            Dimension dimYCubSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, lineY, sketchYLines);
            ElementTransformUtils.MoveElement(doc, dimYCubSketch.Id, new XYZ(deltaX + 500 / 304.8, 0, 0));

            Dimension dimXComSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, lineX, raXInside);
            ElementTransformUtils.MoveElement(doc, dimXComSketch.Id, new XYZ(0, deltaY + 700 / 304.8, 0));
            Dimension dimYComSketch = doc.FamilyCreate.NewDimension(doc.ActiveView, lineY, raYInside);
            ElementTransformUtils.MoveElement(doc, dimYComSketch.Id, new XYZ(deltaX + 700 / 304.8, 0, 0));
            
			dimXCom.AreSegmentsEqual = true;
			dimYCom.AreSegmentsEqual = true;
			dimXComSketch.AreSegmentsEqual = true;
			dimYComSketch.AreSegmentsEqual = true;

			FamilyManager famMan = doc.FamilyManager;
			FamilyParameter W = famMan.AddParameter("W", BuiltInParameterGroup.PG_GENERAL, ParameterType.Length, true);
			Parameter height = cubeExtrusion.get_Parameter(BuiltInParameter.EXTRUSION_END_PARAM);
			dimXCubSketch.FamilyLabel = W;
			dimYCubSketch.FamilyLabel = W;
			dimXCub.FamilyLabel = W;
			dimYCub.FamilyLabel = W;
			famMan.AssociateElementParameterToFamilyParameter(height, W);
        }
    }
}