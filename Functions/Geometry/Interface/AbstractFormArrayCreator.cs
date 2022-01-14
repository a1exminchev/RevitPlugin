using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public abstract class AbstractFormArrayCreator<T> : IFormArrayCreator<T>
    {
        protected readonly Document _Doc;
        protected AbstractFormArrayCreator(Document doc, T Properties)
        {
            doc = _Doc;
            Properties = Props;
        }
        public abstract FormArray Create();
        public T Props { get; set; }
    }
}
