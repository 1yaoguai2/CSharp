using System;

namespace DelegateStudy
{
    public class DelegateClass
    {
        public delegate double Arithmetic(double x, double y);

        //加
        public double Add(double x, double y)
        {
            return x + y;
        }

        //减
        public double Des(double x, double y)
        {
            return x - y;
        }

        //乘
        public double Mul(double x, double y)
        {
            return x * y;
        }

        //除
        public double Div(double x, double y)
        {
            return x / y;
        }

        public static void TestA(Arithmetic arithmeticMethod)
        {
            Console.WriteLine("输入两个数字，进行运算");
            Console.Write("请输入X:");
            double x = Convert.ToDouble(Console.ReadLine());
            Console.Write("请输入y:");
            double y = Convert.ToDouble(Console.ReadLine());
            //委托方法工作，怎么工作由委托决定
            double result = arithmeticMethod(x, y);
            Console.WriteLine("X:{0}与Y:{1}委托方法计算结果为：{2}", x, y, result);
        }
    }


    /*
     * 委托，灵活调用具体实现功能的方法代码，隔离固定代码和变化代码。
     * 例如设计 => 委托生产 => 销售。通过委托生产，达到灵活找对象生产产品，保证产品质量和产量。
     * 利用委托实现封装和隔离，把变化生产的方法当作参数传递给使用生产方法生产的固定方法，让使用生产方法的固定方法不变。
     * 委托定义函数的形状。
     * 定义委托的格式 修饰符 委托 返回类型 委托名称 （参数列表） private delegate string DelegateName (参数 1， 参数 2 ，....)
     * 实例化委托格式 委托类型 委托变量 = new 委托名称 (方法名)/(匿名函数Lamdba表达式); 或者 委托类型 委托变量 = 方法名；
     * 使用委托 委托引用名(实参列表); 或者委托引用?.invoke(参数); 委托方法不一样，执行效果不一样。
     */

    public class ActionOrFuncClass
    {
        public Action<double, double> _action;
        public Func<double, double, double> _func;

        public void WriteResult(double a, double b)
        {
            Console.WriteLine("参数1:{0}和参数2:{1},的和是:{2}", a, b, a + b);
        }
 
        public static void TestB(Action<double, double> action)
        {
            action(10,10);
        }

        public static void TestC(Func<double, double, double> func)
        {
            Console.WriteLine("输入两个数字，进行运算");
            Console.Write("请输入X:");
            double x = Convert.ToDouble(Console.ReadLine());
            Console.Write("请输入y:");
            double y = Convert.ToDouble(Console.ReadLine());
            //委托方法工作，怎么工作由委托决定
            double result = func(x, y);
            Console.WriteLine("X:{0}与Y:{1}委托方法计算结果为：{2}", x, y, result);
        }
    }
    /*
     * 泛型委托，将委托分为两大类，有无返回值，根据带的参数不同和参数数量不同，自定义泛型委托
     * Action 无返回值委托
     * Func 有返回值委托
     */
}