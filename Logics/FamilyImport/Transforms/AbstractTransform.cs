using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Logics.FamilyImport.Transforms
{
    public abstract class AbstractTransform
    {
        public AbstractTransform(){

        }

        public abstract void Create(Document doc);
    }
}
