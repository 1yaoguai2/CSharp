using System;

namespace ReflectionStudy
{
    public class ReflectionTestClass
    {
        private static string 静态私有字段 = "静态私有字段";
        private string 私有字段 = "私有字段";

        private string 私有属性 { get; set; } = "私有属性";

        private void FunA()
        {
            Console.WriteLine("执行私有方法FunA");
        }

        private static void FunB()
        {
            Console.WriteLine("执行静态私有方法FunB");
        }
    }
}