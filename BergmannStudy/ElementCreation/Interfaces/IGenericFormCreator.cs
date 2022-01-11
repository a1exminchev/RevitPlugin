using Autodesk.Revit.DB;

namespace ArCm.ElementCreation.Interfaces{
	public interface IGenericFormCreator<T>{
		GenericForm CreateForm( );
		T           Props { get; set; }
	}
}