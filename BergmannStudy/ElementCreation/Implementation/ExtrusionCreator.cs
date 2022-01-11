using ArCm.ElementCreation.Interfaces;
using Autodesk.Revit.DB;

namespace ArCm.ElementCreation.Implementation{
	public class ExtrusionCreator:AbstractGenericFormCreator<ExtrusionParams>{
		 

		public ExtrusionCreator(Document doc, ExtrusionParams props):base(doc,props) {
			
		}

		public override GenericForm CreateForm() {

 
			return null;
		}
	}

	public class ExtrusionParams{
		public XYZ StartPoint { get; set; }
		public double   Width          { get; set; }
		public double   Height         { get; set; }
	}
}