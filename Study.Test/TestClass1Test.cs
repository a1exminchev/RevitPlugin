using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Study;
using NUnit.Framework;

namespace Study.Test
{
    [TestFixture]
    public class TestClass1Test
    {
        [Test]
        public void Calculator_Plus()
        {
            Calculator calc = new Calculator();

            double a = 6;
            double b = 2;
            double result = calc.Plus(a, b);

            Assert.True(result == 8);
        }

        [Test]
        public void Calculator_Minus()
        {
            Calculator calc = new Calculator();

            double a = 6;
            double b = 2;
            double result = calc.Minus(a, b);

            Assert.True(result == 4);
        }

        [Test]
        public void Calculator_Multiply()
        {
            Calculator calc = new Calculator();

            double a = 6;
            double b = 2;
            double result = calc.Multiply(a, b);

            Assert.True(result == 12);
        }

        [Test]
        public void Calculator_DivideByZero()
        {
            Calculator calc = new Calculator();

            double a = 6;
            double b = 0;
            Assert.Throws<DivideByZeroException>(() => calc.Divide(a, b));

        }
    }
}
