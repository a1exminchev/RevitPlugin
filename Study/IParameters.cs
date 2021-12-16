using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    public interface IParameters
    {
        string Colour { get; set; }
        double Length { get; set; }
        double Points { get; }
    }
}
