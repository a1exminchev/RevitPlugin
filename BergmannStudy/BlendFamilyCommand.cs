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
        static AddInId addId = new AddInId(new Guid("DB33B495-E5B8-4157-B993-F52213964DB0"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            FamilyOperations.CreateNewFamily(app, uiApp, "MyBlend", @"C:\Users\Aleksey Minchev\Desktop\Families\Generic Model.rft");
            uidoc = uiApp.ActiveUIDocument;
            doc = uidoc.Document;
            Transaction t = new Transaction(doc, "blend");
            using (t)
            {
                t.Start();
                Ribbon.MyBlend.MyBlendExecute(doc);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}