using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace Logics.RevitDocument
{
    public class DocCreator
    {
        private readonly Application _app;
        private Document _newDoc;
        public DocCreator(Application app)
        {
            _app = app;
        }
        public Document CreateNewDocument(UIApplication uiApp, string name)
        {
            Document newDoc = _app.NewProjectDocument(UnitSystem.Metric);
            return newDoc;
        }
    }
}
