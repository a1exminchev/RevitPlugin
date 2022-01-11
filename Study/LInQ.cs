using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHome
{
    class LInQ
    {
        static void Main()
        {
            List<string> names = new List<string>() { "David", "Anastasia", "Aleksey", "Nadya" };
            var list = names.Where(x => x.Contains("s")).Select(x => x.ToUpper()); // Методы Where и Select расширяемые для любого типа Enumerable.
                                                                                   // Имена, где переменная Х это каждое имя, которое содержит "s".
                                                                                   // Вторым шагом из оставшихся выбираем каждое имя Х и поднимаем буквы.
            // Можно записать по другому как в SQL
            var list2 = from x in names
                        where x.Contains("s")
                        select x.ToUpper();
            var list3 = names.Where(x => x.Contains("s"))
                             .OrderBy(x => x.Length) //сортирует по длине в порядке возрастания
                             .Select(x => x.ToUpper());
        }
    }
}
