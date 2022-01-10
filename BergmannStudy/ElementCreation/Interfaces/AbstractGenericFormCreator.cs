using ArCm.ElementCreation.Implementation;
using Autodesk.Revit.DB;

namespace ArCm.ElementCreation.Interfaces{
	public abstract class AbstractGenericFormCreator<T> : IGenericFormCreator<T>{
		protected readonly Document _Doc;

		protected AbstractGenericFormCreator(Document doc, T properties) {
			_Doc  = doc;
			Props = properties;
		}

		public abstract GenericForm CreateForm();
		public          T           Props { get; set; }
	}
}