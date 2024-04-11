using System;

namespace AnonymousMethodStudy
{
    public class AnonymousClass
    {
        delegate void NumberChanger(int num);

        private NumberChanger nc = delegate(int x)
        {
            Console.WriteLine("执行了匿名方法：{0}",x);
        };
        //使用Lambda表达式简化 
        private NumberChanger nc1 = x => Console.WriteLine("执行了Lambda表达式简化的匿名方法：{0}", x);

        public void Fun()
        {
            nc(10);
            nc1(11);
        }
    }
}