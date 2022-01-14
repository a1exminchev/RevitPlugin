using System;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Logics.Geometry.Implementation;
using Logics.Geometry.Interface;

namespace StudyTask.Ribbon{
	public class FamilyCopir{
		private readonly Application _app;
		private          Document    _newDoc;

		public FamilyCopir(Application app) {
			_app = app;
		}


		public Document CopyDoc(Document doc) {
			if (!doc.IsFamilyDocument) {
				return null;
			}

			_newDoc = CreateNewDoc(doc);
			if (_newDoc==null) {
				return _newDoc;
			}

			var genericForms = new FilteredElementCollector(doc)
			                   .OfClass(typeof(GenericForm))
			                   .Cast<GenericForm>()
			                   .ToList();
			
			foreach (var genericForm in genericForms) {
				CopyGenericForm(genericForm);
			}

			return _newDoc;

		}

		private GenericForm CopyGenericForm(GenericForm genericForm) {
			var createdForm = genericForm switch{
					Extrusion ex=>CreateExtrusion(ex),
					_=> null
			};

			return createdForm;
		}

		private GenericForm CreateExtrusion(Extrusion extrusion) {
			ExtrusionParameters cubeParameters2 = new ExtrusionParameters();
			cubeParameters2.curveArray = extrusion.Sketch.Profile;


			cubeParameters2.SketchPlane = CreateSketchPlane(extrusion.Sketch);
			cubeParameters2.isSolid     = extrusion.IsSolid;
			
			
			var geometryElementCreator2 = new  ExtrusionCreator(_newDoc, cubeParameters2);
			return geometryElementCreator2.Create() as Extrusion;
		}

		private SketchPlane CreateSketchPlane(Sketch extrusionSketch) {
			return null;
		}

		private Document CreateNewDoc(Document doc) {
			return null;
		}
	}
}