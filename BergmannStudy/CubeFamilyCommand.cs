using System;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace ArCm
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CubeFamilyCommand : IExternalCommand
    {
        static AddInId addId = new AddInId(new Guid("DB33B495-E5B8-4557-B943-F52213965DB0"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            Functions.FamilyOperations.CreateNewFamily(app, uiApp, "MyCube", @"C:\Users\79518\Desktop\Families\Generic Model.rft");
            uidoc = uiApp.ActiveUIDocument;
            doc = uidoc.Document;
            Transaction t = new Transaction(doc, "extrusion");
            using (t)
            {
                t.Start();
                BergmannStudy.Ribbon.MyCube.MyCubeExecute(doc);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}