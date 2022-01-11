using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace Logics.RevitDocument
{
    public class FamilyOperations
    {
        public static void CreateNewFamily(Application app, UIApplication uiApp, string name, string templateFullName)
        {
			string famTemplatePath = app.FamilyTemplatePath;
			Document famDoc = app.NewFamilyDocument(templateFullName);
			string fullName = Path.Combine(famTemplatePath, name + ".rfa");
			if (File.Exists(fullName))
			{
				File.Delete(fullName);
				famDoc.SaveAs(fullName);
				uiApp.OpenAndActivateDocument(fullName);
			}
			else
			{
				famDoc.SaveAs(fullName);
				uiApp.OpenAndActivateDocument(fullName);
			}
		}
    }
}
