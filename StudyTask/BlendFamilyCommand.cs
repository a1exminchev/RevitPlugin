using System;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace StudyTask
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class BlendFamilyCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;

            FamilyCreator familyCreator = new FamilyCreator(app);
            Document newDoc = familyCreator.CreateNewFamily(uiApp, "Blend", "Metric Generic Model");
            Transaction t = new Transaction(newDoc, "Blend");
            using (t)
            {
                t.Start();
                Ribbon.MyBlend.MyBlendExecute(newDoc);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}