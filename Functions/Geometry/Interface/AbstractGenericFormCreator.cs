using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public abstract class AbstractGenericFormCreator<T> : IGenericFormCreator<T>
    {
        protected readonly Document _Doc;
        protected AbstractGenericFormCreator(Document doc, T Properties)
        {
            doc = _Doc;
            Properties = Props;
        }
        public abstract GenericForm Create();
        public T Props { get; set; }
    }
}
