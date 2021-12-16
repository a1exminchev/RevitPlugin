using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    public static class ExtensionMethod
    {
        public static int GetSymbolPosition(this string Text, char Symbol)
        {
            char[] symbols = Text.ToCharArray();
            int i = 0;
            foreach (char c in symbols)
            {
                i++;
                if (c == Symbol)
                {
                    break;
                }
            }
            return i;
        }
    }

    class Program
    {
        void Main()
        {
            string a = "Good morning";
            a.GetSymbolPosition('n'); //Расширяемый метод
        }
    }
}
