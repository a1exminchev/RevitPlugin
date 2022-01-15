using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Logics.Geometry.Implementation;

namespace Logics.RevitDocument
{
    public class FamilyCopier
    {
        private readonly Application _app;
        private readonly UIApplication _uiApp;
        private Document _newDoc;
        public FamilyCopier(Application app, UIApplication uiApp)
        {
            _app = app;
            _uiApp = uiApp;
        }
        public Document CopyFamilyDoc(Document doc)
        {
            if (!doc.IsFamilyDocument)
            {
                return null;
            }

            _newDoc = CreateNewDoc(doc);
            if (_newDoc == null)
            {
                return _newDoc;
            }

            var genericForms = new FilteredElementCollector(doc)
                                .OfClass(typeof(GenericForm))
                                .Cast<GenericForm>()
                                .ToList();
            Transaction t = new Transaction(_newDoc, "Copy Family");
            using (t)
            {
                t.Start();
                foreach (var genericForm in genericForms)
                {
                    CopyGenericFormToNewDoc(genericForm);
                }
                t.Commit();
            }

            return _newDoc;
        }

        private GenericForm CopyGenericFormToNewDoc(GenericForm genericForm)
        {
            GenericForm createdForm;
            switch (genericForm.GetType().ToString())
            {
                case "Autodesk.Revit.DB.Extrusion":
                    createdForm = CreateExtrusionInNewDoc(genericForm as Extrusion);
                    return createdForm;
                case "Autodesk.Revit.DB.Blend":
                    createdForm = CreateBlendInNewDoc(genericForm as Blend);
                    return createdForm;
                case "Autodesk.Revit.DB.Sweep":
                    createdForm = CreateSweepInNewDoc(genericForm as Sweep);
                    return createdForm;
                case "Autodesk.Revit.DB.SweptBlend":
                    createdForm = CreateSweptBlendInNewDoc(genericForm as SweptBlend);
                    return createdForm;
                case "Autodesk.Revit.DB.Revolution":
                    createdForm = CreateRevolutionInNewDoc(genericForm as Revolution);
                    return createdForm;
            }
            return null;
        }

        private GenericForm CreateExtrusionInNewDoc(Extrusion extrusion)
        {
            ExtrusionParameters extrusionParameters = new ExtrusionParameters();
            extrusionParameters.isSolid = extrusion.IsSolid;
            extrusionParameters.curveArrArray = extrusion.Sketch.Profile;
            extrusionParameters.SketchPlane = extrusion.Sketch.SketchPlane;
            extrusionParameters.Height = extrusion.EndOffset;
            var geometryElementCreator = new ExtrusionCreator(_newDoc, extrusionParameters);
            return geometryElementCreator.Create() as Extrusion;
        }
        private GenericForm CreateBlendInNewDoc(Blend blend)
        {
            BlendParameters blendParameters = new BlendParameters();
            blendParameters.isSolid = blend.IsSolid;
            blendParameters.BaseCurveArray = TakeSingleCurveArray(blend.BottomProfile);
            blendParameters.TopCurveArray = TakeSingleCurveArray(blend.TopProfile);
            blendParameters.BaseSketchPlane = blend.BottomSketch.SketchPlane;
            var geometryElementCreator = new BlendCreator(_newDoc, blendParameters);
            return geometryElementCreator.Create() as Blend;
        }
        private GenericForm CreateSweepInNewDoc(Sweep sweep)
        {
            SweepParameters sweepParameters = new SweepParameters();
            sweepParameters.isSolid = sweep.IsSolid;
            sweepParameters.ProfileCurveArrArray = sweep.ProfileSketch.Profile;
            sweepParameters.PathCurveArray = TakeSingleCurveArray(sweep.PathSketch.Profile);
            sweepParameters.PathSketchPlane = sweep.PathSketch.SketchPlane;
            sweepParameters.WhichPathLineIsForProfile = 0;
            var geometryElementCreator = new SweepCreator(_newDoc, sweepParameters);
            return geometryElementCreator.Create() as Sweep;
        }
        private GenericForm CreateSweptBlendInNewDoc(SweptBlend sweptblend)
        {
            SweptBlendParameters sweptBlendPar = new SweptBlendParameters();
            sweptBlendPar.isSolid = sweptblend.IsSolid;
            Options op = new Options();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;
            sweptBlendPar.PathCurve = TakeSingleCurveArray(sweptblend.PathSketch.Profile).get_Item(0);
            sweptBlendPar.PathSketchPlane = sweptblend.PathSketch.SketchPlane;
            sweptBlendPar.Profile1CurveArray = TakeSingleCurveArray(sweptblend.BottomProfile);
            sweptBlendPar.Profile2CurveArray = TakeSingleCurveArray(sweptblend.TopProfile);
            var geometryElementCreator = new SweptBlendCreator(_newDoc, sweptBlendPar);
            return geometryElementCreator.Create() as SweptBlend;
        }
        private GenericForm CreateRevolutionInNewDoc(Revolution revolution)
        {
            RevolutionParameters revolutionParameters = new RevolutionParameters();
            revolutionParameters.isSolid = revolution.IsSolid;
            revolutionParameters.Axis = revolution.Axis.GeometryCurve as Line;
            revolutionParameters.SketchPlane = revolution.Sketch.SketchPlane;
            revolutionParameters.ProfileCurveArrArray = revolution.Sketch.Profile;
            revolutionParameters.StartingAngle = revolution.StartAngle;
            revolutionParameters.EndingAngle = revolution.EndAngle;
            var geometryElementCreator = new RevolutionCreator(_newDoc, revolutionParameters);
            return geometryElementCreator.Create() as Revolution;
        }

        private CurveArray TakeSingleCurveArray(CurveArrArray curveArrArray)
        {
            foreach (CurveArray curArr in curveArrArray)
            {
                return curArr;
            }
            return null;
        }

        private Document CreateNewDoc(Document doc)
        {
            FamilyCreator familyCreator = new FamilyCreator(_app);
            string templatePath = Path.Combine(_app.FamilyTemplatePath, "Generic Model.rft");
            TaskDialog.Show("F", templatePath);
            _newDoc = familyCreator.CreateNewFamily(_uiApp, "New Family Document", templatePath);
            return _newDoc;
        }
    }
}
