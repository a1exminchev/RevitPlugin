using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace StudyHome
{
    public class LineObject : IUserActions, IParameters
    {
        static string colour;
        static double points; //will be counted by instance
        static double length; //this will be length of created by user line

        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        public double Length
        {
            get { return length; }
            set { length = value; }
        }
        public double Points
        {
            get { return points; }
        }
        public void MoveRight(double distance)
        {
            TaskDialog.Show("It moved by", distance.ToString()); //moving right by distance
        }
    }
}
