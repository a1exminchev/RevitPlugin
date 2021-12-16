using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    //Ниже лямбда выражения
    class Lambda
    {
        public delegate void Operator(); //Объявляем делегат для методов без входящих аргументов
        static void Main()
        {
            Operator op = () => Console.WriteLine("Hello!"); //создаем экземпляр делегата, вводим аргументы (),
                                                             //вводим "выражение лямбды"- анонимную функцию,
                                                             //которая присваивается экземпляру делегата
            
            op(); //обращаемся к выполнению делегата, который уже содержит в себе выражение лямбды

            Console.ReadKey();
        }

        public delegate bool OperatorCompare(int a, int b); //Объявляем делегат для сравнительных методов, который возвращает bool
        static void Main2()
        {
            OperatorCompare opC = (x, y) => x == y; //создаем экземпляр делегата, вводим аргументы (int a, int b) переменными,
                                                    //вводим выражение лямбды, являющееся скрытым методом, выдающим
                                                    //значение bool. Если тело выражения (х==у) (не)верно, то выдается true(false)

            bool result = opC(9, 10); //обращаемся к методу делегата, который уже содержит в себе выражение лямбды
            Console.WriteLine(result.ToString());
            Console.ReadKey();
        }

        //Ниже лямбда-операторы
        public delegate void Operator2(string s); //Объявляем делегат для сравнительных методов, который возвращает bool
        static void Main3()
        {
            Operator2 op2 = (x) => { Console.WriteLine("Hello" + x); }; //создаем экземпляр делегата, вводим аргументы (int a, int b) переменными,
                                                    //вводим выражение лямбды, являющееся скрытым методом, выдающим
                                                    //значение bool. Если тело выражения (х==у) (не)верно, то выдается true(false)

            op2(", World!"); //обращаемся к методу делегата, который уже содержит в себе выражение лямбды
            Console.ReadKey();
        }

        public delegate string Operator3(string s);
        static void Main4()
        {
            Operator3 op3 = (x) => { string res = "Hello" + x; return res; };

            op3(", World!");
            Console.ReadKey();
        }
        static void Main5()
        {
            string text = new Operator3((x) => { string res = "Hello" + x; return res; }).Invoke(", World!");

            Console.ReadKey();
        }

        // В Ревит поиске в фильтрах применяется так:
        // SomeFilter.Where(x => x.Name == "BlueWindow") Читается как фильтруем", где объект Х, у которого Х.Имя равно "BlueWindow"".
    }
}
