using System;

namespace StructStudy
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //结构是值类型，分配到栈上
            StructTest2 structTest2 = new StructTest2(1, 2);
            StructTest2 structTest21 = structTest2; //复制整个结构，是全新的结构体
            
            //类是引用类型分配到堆上
            MyClass myClass = new MyClass(1,2);
            MyClass myClass2 = myClass;  //复制引用，指向同一个对象，除非使用jsongconvert序列化和反序列化。

            //修改值
            structTest21.x = 3;
            //修改结构实例，不影响其他实列
            Console.WriteLine(structTest2.x);
            myClass2.x = 3;
            //修改类实例会影响其他实列
            Console.WriteLine(myClass.x);

            Books book1 = new Books();
            Books book2 = new Books();
            Books book11 = book1;
            
            book1.setValues("C","AA","sss",4550);
            book2.setValues("ts","aa+","ss++",56660);
            
            book11.setValues("ts11","a11++","s11++",11111);
            
            book1.Display();
            book2.Display();
            book11.Display();

            //-- 结果
            // 1
            // 3
            // Title:C
            // Author:AA
            // Subject:sss
            // Book_id:4550
            // Title:ts
            // Author:aa+
            //     Subject:ss++
            // Book_id:56660
            // Title:ts11
            // Author:a11++
            // Subject:s11++
            // Book_id:11111

        }

        struct Books
        {
            private string title;
            private string author;
            private string subject;
            private int book_id;

            public void setValues(string t , string a ,string s,int id)
            {
                title = t;
                author = a;
                subject = s;
                book_id = id;
            }

            public void Display()
            {
                Console.WriteLine("Title:{0}",title);
                Console.WriteLine("Author:{0}",author);
                Console.WriteLine("Subject:{0}",subject);
                Console.WriteLine("Book_id:{0}",book_id);
            }
        }
        public class  MyClass
        {
            public int x;
            public int y;

            public MyClass(int m ,int n)
            {
                x = m;
                y = n;
            }

        }
    }
    
    /*
     * 结构体与类的区别
     * 类和结构有以下几个基本的不同点：

值类型 vs 引用类型：

结构是值类型（Value Type）： 结构是值类型，它们在栈上分配内存，而不是在堆上。当将结构实例传递给方法或赋值给另一个变量时，将复制整个结构的内容。
类是引用类型（Reference Type）： 类是引用类型，它们在堆上分配内存。当将类实例传递给方法或赋值给另一个变量时，实际上是传递引用（内存地址）而不是整个对象的副本。
继承和多态性：

结构不能继承： 结构不能继承其他结构或类，也不能作为其他结构或类的基类。
类支持继承： 类支持继承和多态性，可以通过派生新类来扩展现有类的功能。
默认构造函数：

结构不能有无参数的构造函数： 结构不能包含无参数的构造函数。每个结构都必须有至少一个有参数的构造函数。
类可以有无参数的构造函数： 类可以包含无参数的构造函数，如果没有提供构造函数，系统会提供默认的无参数构造函数。
赋值行为：

类型为类的变量在赋值时存储的是引用，因此两个变量指向同一个对象。
结构变量在赋值时会复制整个结构，因此每个变量都有自己的独立副本。
传递方式：

类型为类的对象在方法调用时通过引用传递，这意味着在方法中对对象所做的更改会影响到原始对象。
结构对象通常通过值传递，这意味着传递的是结构的副本，而不是原始结构对象本身。因此，在方法中对结构所做的更改不会影响到原始对象。
可空性：

结构体是值类型，不能直接设置为 null：因为 null 是引用类型的默认值，而不是值类型的默认值。如果你需要表示结构体变量的缺失或无效状态，可以使用 Nullable<T> 或称为 T? 的可空类型。
类默认可为null： 类的实例默认可以为 null，因为它们是引用类型。
性能和内存分配：

结构通常更轻量： 由于结构是值类型且在栈上分配内存，它们通常比类更轻量，适用于简单的数据表示。
类可能有更多开销： 由于类是引用类型，可能涉及更多的内存开销和管理。
     *
     * 
     */
}