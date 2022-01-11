using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHome
{
    public class Calculator
    {
		public double Plus(double a, double b)
		{
			double result = a + b;
			return result;
			throw new InvalidOperationException("Вводные данные некорректные");
		}
		public double Minus(double a, double b)
		{
			double result = a - b;
			return result;
			throw new InvalidOperationException("Вводные данные некорректные");
		}

		public double Divide(double a, double b)
		{
			if (b == 0)
            {
				throw new DivideByZeroException("Деление на ноль запрещено");
			}
			double result = a / b;
			return result;
			throw new InvalidOperationException("Вводные данные некорректные");
		}

		public double Multiply(double a, double b)
		{
			double result = a * b;
			return result;
			throw new InvalidOperationException("Вводные данные некорректные");
		}
	}
}
