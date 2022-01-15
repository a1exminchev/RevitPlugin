using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace Logics.RevitDocument
{
    public class FamilyCreator
    {
        private readonly Application _app;
        private Document _newDoc;
        public FamilyCreator(Application app)
        {
            _app = app;
        }
        public Document CreateNewFamily(UIApplication uiApp, string name, string templateFullName)
        {
            string famTemplatePath = _app.FamilyTemplatePath;
            Document newFamDoc = _app.NewFamilyDocument(templateFullName);
            string fullName = Path.Combine(famTemplatePath, name + ".rfa");
            if (File.Exists(fullName))
            {
                try
                {
                    File.Delete(fullName);
                    newFamDoc.SaveAs(fullName);
                    uiApp.OpenAndActivateDocument(fullName);
                }
                catch (IOException) { }
            }
            else
            {
                newFamDoc.SaveAs(fullName);
                uiApp.OpenAndActivateDocument(fullName);
            }
            return newFamDoc;
        }
    }
}
