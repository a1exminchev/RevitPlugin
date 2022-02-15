using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Logics.Import.Transforms
{
    public abstract class AbstractTransfer
    {
        public AbstractTransfer(){

        }

        public abstract void Create(Document doc);
    }
}
