using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public interface IGenericFormCreator<T>
    {
        GenericForm Create();
        T Props { get; set; }
    }
}