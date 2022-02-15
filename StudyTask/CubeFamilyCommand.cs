using System;
using Logics.RevitDocument;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using SCOPE_RevitPluginLogic.Utils;

namespace StudyTask
{
    [TransactionAttribute(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CubeFamilyCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;
            Configure.ConfigureLogger();

            FamilyCreator familyCreator = new FamilyCreator(app);
            Document newDoc = familyCreator.CreateNewFamily(uiApp, "MyCubes", "Metric Generic Model");
            Transaction t = new Transaction(newDoc, "extrusion");
            using (t)
            {
                t.Start();
                Ribbon.MyCubes.MyCubesExecute(newDoc);
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}