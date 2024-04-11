using System;

namespace LambdaStudy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
           Console.WriteLine("打印Lambda表达式学习日志：");
           LambdaClass lambdaClass = new LambdaClass();
           lambdaClass.LambdaTestMothed();
           Console.WriteLine("");
           
           Console.WriteLine("打印Lambda表达式实现闭包学习日志：");
           Console.WriteLine("打印无状态函数：");
           Console.WriteLine("Hello World");
           Console.WriteLine("你好，世界");
           Console.WriteLine("打印有状态的函数：");
           StatusMethodClass statusMethodClass = new StatusMethodClass();
           statusMethodClass.StatusMethod("Hello World");
           statusMethodClass.StatusMethod(null);
           statusMethodClass.StatusMethod("你好，世界");
           statusMethodClass.StatusMethod(null);
           
           Console.WriteLine("打印Lambda实现的闭包方法：");
           var closureMethod = new ClosureClass().CreateWrite();
           closureMethod("Hello World");
           closureMethod(null);
           closureMethod("你好，世界");
           closureMethod(null);
           
           Console.WriteLine("打印Lambda表达式的用用途--Linq扩展，Where语句");
           WhereStudy whereStudy = new WhereStudy();
           whereStudy.WhereMethod();
           
            Console.ReadKey();
        }

       
    }
    
    
}