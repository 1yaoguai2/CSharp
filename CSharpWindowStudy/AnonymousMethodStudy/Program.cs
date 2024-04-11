using System;
using System.Data.SqlClient;

namespace AnonymousMethodStudy
{
    internal class Program
    {
        delegate void NumberTest(int i);

        public static void Main(string[] args)
        {
            AnonymousClass anonymousClass = new AnonymousClass();
            anonymousClass.Fun();

            Console.WriteLine("*******执行匿名方法创建委托实例测试");
            NumberTest numberTest = delegate(int i)
            {
                Console.WriteLine("执行匿名方法创建的实例委托，{0}", i);
            };
            //使用Lambda表达式简化
            NumberTest numberTest1 = i => Console.WriteLine("执行Lambda表达式简化匿名方法创建的实例委托，{0}", i);
            //使用匿名方法
            numberTest(TestStaticClass.num);
            numberTest1(TestStaticClass.num);

            //使用匿名方法实例化委托
            numberTest = new NumberTest(TestStaticClass.AddNum);
            numberTest(TestStaticClass.num); //使用命名方法调用委托

            //使用另一个命名方法实例化委托
            numberTest = new NumberTest(TestStaticClass.MultNum);
            numberTest(TestStaticClass.num);

            Console.ReadKey();
        }
    }
    
    /* 匿名方法（Anonymous method） 提供了一种传递代码块作为委托参数的技术。匿名方法是没有名称只有主体的方法。
       在匿名方法中您不需要指定返回类型，它是从方法主体内的 return 语句推断的。
       
       特殊方法，没有名称，不需要返回类型，方法主体中推断是否有返回值和返回值类型，可以实例化委托。调用委托时，执行方法。
    
    */
}