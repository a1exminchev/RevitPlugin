using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public interface IFormArrayCreator<T>
    {
        FormArray Create();
        T Props { get; set; }
    }
}