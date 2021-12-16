using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study
{
    class Delegate
    {
        delegate double commonOperator(double a, double b);
        public Action<double, double> verycommonOperator; //Может взаимодействовать с любым выходным типом

        static double Sum(double a, double b)
        {
            return (a + b);
        }
        static double Minus(double a, double b)
        {
            return (a - b);
        }
        static double Div(double a, double b)
        {
            return (a / b);
        }
        static double Mult(double a, double b)
        {
            return (a * b);
        }
        
        static void Main()
        {
            commonOperator operation = new commonOperator(Sum);

            double a = 3;
            double b = 5;
            Console.WriteLine(operation(a, b));
            Console.ReadLine(); //Выдаст сумму
            operation = new commonOperator(Minus); //Выдаст разницы
            commonOperator operation2 = Mult; //второй вариант присвоения
            commonOperator Divide = new commonOperator(Div); //Выдаст деление
        }
    }
}
