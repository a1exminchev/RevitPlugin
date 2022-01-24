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
    public class CopyFamilyCommand : IExternalCommand
    {
        //static AddInId addId = new AddInId(new Guid("DB63B215-E5B8-4157-B943-F52213728DB0"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var app = commandData.Application.Application;
            var uidoc = uiApp.ActiveUIDocument;
            var doc = uidoc.Document;

            FamilyCopier familyCopier = new FamilyCopier(app, uiApp);
            familyCopier.CopyFamilyDoc(doc);

            return Result.Succeeded;
        }
    }
}