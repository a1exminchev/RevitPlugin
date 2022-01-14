﻿using System;
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
    public class SweepFamilyCommand : IExternalCommand
    {
        static AddInId addId = new AddInId(new Guid("DB73B495-E5B8-4157-B993-F51213954DB0"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            FamilyOperations.CreateNewFamily(app, uiApp, "MySweep", @"C:\Users\Aleksey Minchev\Desktop\Families\Generic Model.rft");
            uidoc = uiApp.ActiveUIDocument;
            doc = uidoc.Document;
            Transaction t = new Transaction(doc, "Sweep");
            using (t)
            {
                t.Start();
                Ribbon.MySweep.MySweepExecute(doc);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}