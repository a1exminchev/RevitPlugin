using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public abstract class AbstractGeometryElementCreator<T> : IGeometryElementCreator<T>
    {
        protected readonly Document _Doc;
        protected AbstractGeometryElementCreator(Document doc, T Properties)
        {
            doc = _Doc;
            Properties = Props;
        }
        public abstract GenericForm Create();
        public T Props { get; set; }
    }
}
