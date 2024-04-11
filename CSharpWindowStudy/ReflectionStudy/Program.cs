using System;
using System.Reflection;
using ReflectionStudy;

namespace ReflecationStudy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var reflecationTestClass = new ReflectionTestClass();
            //reflecationTestClass. 无法获取任何私有成员
            var type = reflecationTestClass.GetType();

            Console.WriteLine("*******获取私有成员*****");
            //枚举类型，赋值实列成员+私有变量；
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            //获取私有字段并改值
            var fieldInfo = type.GetField("私有字段", bindingFlags);
            Console.WriteLine(fieldInfo?.GetValue(reflecationTestClass));
            fieldInfo?.SetValue(reflecationTestClass, "新的私有字段");
            Console.WriteLine(fieldInfo?.GetValue(reflecationTestClass));

            //获取私有属性并改值
            var propertyInfo = type.GetProperty("私有属性", bindingFlags);
            Console.WriteLine(propertyInfo?.GetValue(reflecationTestClass));
            propertyInfo?.SetValue(reflecationTestClass, "新的私有属性");
            Console.WriteLine(propertyInfo?.GetValue(reflecationTestClass));

            //执行私有方法
            var method = type.GetMethod("FunA", bindingFlags);
            method?.Invoke(reflecationTestClass, null);

            Console.WriteLine("*******获取静态私有成员*****");
            //获取静态私有成员
            bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;

            fieldInfo = type.GetField("静态私有字段", bindingFlags);
            Console.WriteLine(fieldInfo?.GetValue(null));

            method = type.GetMethod("FunB", bindingFlags);
            method?.Invoke(reflecationTestClass, null); //传入实例执行静态方法
            method?.Invoke(null, null); //不传入实例执行静态方法
        }
        /* 私有属性不属于实列对象，属于类 */
        /* 调试场景，测试代码中推荐使用反射获取私有变量 */
    }
}