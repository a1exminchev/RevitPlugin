using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHome
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double StartOffset { get; set; }
        public Dictionary<string, List<double>> SketchPlane { get; set; }
        public Dictionary<string, List<double>> CurveArrArray { get; set; }
    }
}
