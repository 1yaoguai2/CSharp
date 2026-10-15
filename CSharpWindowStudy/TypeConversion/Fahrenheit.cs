using System;
using System.Collections.Generic;
using System.Text;

namespace TypeConversion
{
    //华氏温度计
    public class Fahrenheit
    {
        public double Degress { get; set; }

        public Fahrenheit(double degress)
        {
            Degress = degress;
        }

        //隐式转换从Fahrenheit到double
        public static implicit operator Double(Fahrenheit f)
        {
            return f.Degress;
        }

        //显示转换从double到Fahrenheit
        public static explicit operator Fahrenheit(Double d)
        {
            return new Fahrenheit(d);
        }

        public void TestConversion()
        {
            Fahrenheit f = new Fahrenheit(98.6);
            Console.WriteLine($"Fahrenheit: {f.Degress}");
            //隐式转换从Fahrenheit到double
            double d = f; // Implicit conversion from Fahrenheit to double
            Console.WriteLine($"Implicit conversion: {f.Degress} Fahrenheit is {d} in double.");
            //显示转换从double到Fahrenheit
            double d2 = 100.0;
            Fahrenheit f2 = (Fahrenheit)d2; // Explicit conversion from double to Fahrenheit
            Console.WriteLine($"Explicit conversion: {d2} in double is {f2.Degress} Fahrenheit.");
        }
    }
}

/*
 * Implicit和Explicit转换运算符的使用示例：
 * 通过自定义隐式和显式转换运算符，让 Fahrenheit 类既能保持面向对象封装，又能像基础类型一样自然、安全、可读地进行类型转换。
 * 类型之间可以无缝互转
 * 转换过程是可控的、可预期的
 * 不会破坏封装性（内部字段 Degrees 仍被保护）
 */
