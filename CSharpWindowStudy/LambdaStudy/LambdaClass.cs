using System;
using System.Linq;

namespace LambdaStudy
{
    public class LambdaClass
    {
        public void LambdaTestMothed()
        {
            //无参无返回值匿名函数
            Action action = () => { Console.WriteLine("无参无返回匿名函数。"); };
            action();
            //无参无返回值Lambda表达式
            Action action1 = () => Console.WriteLine("无参无返回值匿名函数Lambda表达式。");
            action1();

            //有参无返回值的匿名函数
            Action<int> action2 = (int i) => { Console.WriteLine("有参数无返回值的匿名函数，参数：{0}", i); };
            action2(2);
            //有参数无返回值的Lambda表达式
            Action<int> action3 = i => Console.WriteLine("有参数无返回值的匿名函数函数Lambda表达式，参数：{0}", i);
            action3(3);

            //有参有返回值的匿名函数
            Func<int, int> func = (int x) => { return x + 1; };
            Console.WriteLine("有参有返回值的匿名函数，参数{0}，返回值{1}", 99, func(99));

            //有参有返回值的匿名函数的Lambda表达式
            Func<int, int> func1 = x => x + 1;
            Console.WriteLine("有参有返回值的匿名函数的Lambda表达式，参数{0}，返回值{1}", 99, func1(99));

            //Lambda作为参数
            int[] arr = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine($"arr数组中有{arr.Count(x => x % 2 == 1)}个奇数");
        }

        /* 一种简写的语法，省空间，省代码，好看，
         单独的Lambda无法使用，需要配合委托和事件使用
         （参数列表） =>
         {
             //函数主体
         };
         优点：省略了方法名称，Lambda是匿名函数的一种表达式，
         省略了方法的返回值类型，其类型由函数主题实际值返回推断。

         用法：通常参数可以省略类型说明，因为委托中已经定义了参数类型，
         如果只有一个参数可以省略（），
         如果方法体只有一句可以省略{}，
         如果方法体只有一句可以省略return。
         */
    }
}