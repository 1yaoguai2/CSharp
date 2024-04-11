using System;

namespace AnonymousMethodStudy
{
    public class TestStaticClass
    {
        public static int num = 10;

        public static void AddNum(int p)
        {
            num += p;
            Console.WriteLine("执行了方法AddNum,Num={0}",num);
        }

        public static void MultNum(int p)
        {
            num *= p;
            Console.WriteLine("执行了方法MultNum，Num={0}",num);
        }
    }
}