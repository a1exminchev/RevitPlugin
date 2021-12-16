using System;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    class EventClass
    {
        public delegate void MethodContainer();
        public event MethodContainer CountEvent;

        public class Counter
        {
            public void Count() //счётчик от 0 до 4
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i == 2)
                    {
                        EventClass eventClass = new EventClass();
                        eventClass.CountEvent();
                    }
                }
            }
        }

        public class Handler1
        {
            public void Message()
            {
                TaskDialog.Show("Title", "Я зафиксировался");
            }
        }

        public class Handler2
        {
            public void Message()
            {
                TaskDialog.Show("Title", "Я зафиксировался 2");
            }
        }
    }
}