using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;

namespace Study
{
	[TransactionAttribute(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class Class1 : IExternalCommand
	{
		static AddInId addId = new AddInId(new Guid("DB37B475-E5B8-4517-B984-F52213967DB0"));
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			var result = Result.Succeeded;

			//WhiteStraightLine line1 = new WhiteStraightLine();
			//TaskDialog.Show("Hi", line1.Colour);
			//line1.MoveRight(6);

			EventClass.Counter counter = new EventClass.Counter();
			EventClass.Handler1 handler1 = new EventClass.Handler1();
			EventClass.Handler2 handler2 = new EventClass.Handler2();
			EventClass eventClass = new EventClass();
            //Подписываемся на события
            eventClass.CountEvent += handler1.Message;
			eventClass.CountEvent += handler2.Message;
			counter.Count();

			return result;
		}
	}
}
