using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> names = new List<string>() { "David", "Anastasia", "Aleksey", "Nadya" };
            var list = names.Where(x => x.Contains("s")).Select(x => x.ToUpper());

            // Можно записать по другому как в SQL
            var list2 = from x in names
                        where x.Contains("s")
                        select x.ToUpper();

            var list3 = names.Where(x => x.Contains("s"))
                             .OrderBy(x => x.Length) //сортирует по длине в порядке возрастания
                             .Select(x => x.ToUpper());

            foreach (string i in list3)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }
    }
}
