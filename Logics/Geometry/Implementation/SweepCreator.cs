using Autodesk.Revit.DB;
using Logics.Geometry.Interface;
using System;

namespace Logics.Geometry.Implementation
{
    public class SweepCreator : AbstractGenericFormCreator<SweepParameters>
    {
        protected readonly Document FamDoc;
        private readonly SweepParameters _props;


        public SweepCreator(Document doc, SweepParameters properties) : base(doc, properties)
        {
            if (doc.IsFamilyDocument)
            {
                FamDoc = doc;
            }
            _props = properties;
        }
        public override GenericForm Create()
        {
            Sweep sweep = null;
            if (FamDoc != null)
            {
                SweepProfile profile = FamDoc.Application.Create.NewCurveLoopsProfile(_props.ProfileCurveArrArray);
                try
                {
                    sweep = FamDoc.FamilyCreate.NewSweep(_props.isSolid, _props.PathCurveArray, _props.PathSketchPlane, profile, _props.WhichPathLineIsForProfile, ProfilePlaneLocation.Start);
                }
                catch{ }
                Line axisX = Line.CreateBound(XYZ.Zero, XYZ.BasisX);
                Line axisY = Line.CreateBound(XYZ.Zero, XYZ.BasisY);
                if (_props.AngleFromXZtoY != 0)
                {
                    ElementTransformUtils.RotateElement(FamDoc, sweep.PathSketch.SketchPlane.Id, axisX, _props.AngleFromXZtoY);
                }
                if (_props.AngleFromYZtoX != 0)
                {
                    ElementTransformUtils.RotateElement(FamDoc, sweep.PathSketch.SketchPlane.Id, axisY, _props.AngleFromYZtoX);
                }
            }
            return sweep;
        }
    }
    public class SweepParameters
    {
        public bool isSolid { get; set; }
        public CurveArray PathCurveArray { get; set; }
        public CurveArrArray ProfileCurveArrArray { get; set; }
        public int WhichPathLineIsForProfile { get; set; }
        public SketchPlane PathSketchPlane { get; set; }
        public double AngleFromXZtoY { get; set; }
        public double AngleFromYZtoX { get; set; }
        public SketchPlane ProfileSketchPlane { get; set; }
    }
}