using System;

namespace LambdaStudy
{
    public class ClosureClass
    {
        public Action<string?> CreateWrite()
        {
            string msg = "";
            return (string? info) =>
            {
                msg = info ?? msg;
                Console.WriteLine(msg);
            };
        }
    }
    /* 闭包是一种语言特性，运行函数内部定义的函数访问外部函数的局部变量。
     * 即使外层函数已经停止，依然可以使用lambda来实现闭包。
     * 变量在函数内部，提高变量的封装性，内部函数可以把外部函数的局部变量变为自己的成员级，形成一个对象。
     */
}