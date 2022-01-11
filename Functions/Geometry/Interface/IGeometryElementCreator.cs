using System;
using Autodesk.Revit.DB;

namespace Logics.Geometry.Interface
{
    public interface IGeometryElementCreator<T>
    {
        GenericForm Create();
        T Props { get; set; }
    }
}