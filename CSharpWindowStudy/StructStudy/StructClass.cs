namespace StructStudy
{
    public class StructClass
    {
        /// <summary>
        /// 若类是引用类型，栈属于类的字段，存储在堆上
        /// </summary>
        struct StructTest
        {
            public int id;
            private string info;
            public const string _name = "struct";
        }
        /// <summary>
        /// 只读不可修改属性
        /// </summary>
        readonly struct StructTest1
        {
            
        }
    }

    /// <summary>
    ///  存储在栈上
    /// </summary>
    struct  StructTest2
    {
        public int x;
        public int y;
        //public int s = 10; //非Static结构无法赋予初值

        //不能有无参构造函数
        //public StructTest2()
        //{
            
        //}
        //构造函数中必须为所有的字段赋值
        public StructTest2(int m,int n)
        {
            x = m;
            y = n;
        }
        //构造函数不能继承
        //struct myStruct : StructTest2
        //{
        //}
    }
    
    /*
     * 结构的特点
     * 结构提供了一种轻量级的数据类型，适用于表示简单的数据结构，具有较好的性能特性和值语义。
     * 结构可带有方法、字段、索引、属性、运算符方法和事件，适用于表示轻量级数据的情况。
     * 结构可定义有参数的构造函数，不能定义解析,也可以没有构造函数。
     * 结构无法继承和被继承。结构成员不能指定为abstract,virtual,protected
     * 结构可实现一个或者多个接口。
     * 使用new操作符创建结构时，会调用适当的构造函数来创建结构，不适用时在所有字段都被初始化后，字段才被赋值、对象才被使用。
     * 结构变量通常分配在栈上，创建和销毁更快，但是结构属于类的字段，且类为引用类型，那么结构将存储在堆上。
     * 结构字段可定义为只读，此时字段无法修改。
     */
}