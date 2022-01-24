using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Logics.Geometry
{
    public class GeometryOperations
    {
		const double _eps = 1.0e-9;
		static public bool isParallel(XYZ a, XYZ b)
		{
			double angle = a.AngleTo(b);
			return (_eps > angle) || (Math.Abs(angle - Math.PI) < _eps);
		}
	}
}
