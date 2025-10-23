// See https://aka.ms/new-console-template for more information
/* 装箱和拆箱
 * 简单描述：装箱是将一个值类型变量的数据存储在一个引用类型的变量中，反过来，将引用类型中的值转化回栈中的响应的值类型，则为拆箱。
 */

using System.Collections;

public class Program
{
    static void Main()
    {
        object boxed = SimpleBoxing();
        int targetInt = SimpleUnboxing(boxed);
        Console.WriteLine($"拆箱后得到{targetInt.GetType()}类型数据：{targetInt}");
        long myLong = 2;
        object boxed2 = myLong;
        int targetInt2 = SimpleUnboxing(boxed2);

        Console.WriteLine($"--- 测试ArrayList ---");
        WorkWithArrayList();
        Console.ReadLine();
    }

    /// <summary>
    /// 装箱简单演示
    /// 装箱的过程就是将一个值类型分配给 Object 类型变量的过程。
    /// 当你装箱一个值时，CoreCLR 会在堆上分配一个新的对象，并将该值类型的值复制到该对象实例。
    /// 返回给你的是一个在托管堆中新分配的对象的引用。
    /// </summary>
    /// <returns></returns>
    public static object SimpleBoxing()
    {
        int myInt = 10;
        object boxedInt = myInt;

        return boxedInt;
    }


    /// <summary>
    /// 简单拆箱演示
    /// CoreCLR 首先验证接收的数据类型是否等同于被装箱的类型，如果是，它就把值复制回基于栈存储的本地变量中。
    /// 如果你试图将一块数据拆箱到不正确的数据类型中，将会抛出 InvalidCastException 异常。
    /// </summary>
    /// <param name="boxInt"></param>
    /// <returns></returns>
    public static int SimpleUnboxing(object boxInt)
    {
        try
        {
            int myInt = (int)boxInt;
            return myInt;
        }
        catch(InvalidCastException ex)
        {
            Console.WriteLine($"拆箱失败，数据原类型不是Int:{ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// ArrayList
    /// 如果你想使用索引器从 ArrayList 中检索一条数据，你必须使用转换操作将堆分配的对象拆箱为栈分配的整型，
    /// 因为 ArrayList 的索引器返回的是 Object 类型，而不是 int 类型。
    /// </summary>
    static void WorkWithArrayList()
    {
        // 当传递给对象的方法时，值类型会自动被装箱
        ArrayList myArrayList = new ArrayList();
        myArrayList.Add(SimpleBoxing());
        myArrayList.Add(17.0f);
        myArrayList.Add(24.44);
        myArrayList.Add("333");
        myArrayList.Add(true);

        foreach (var item in myArrayList)
        {
            //item是object
            Console.WriteLine(item);
        }

        //拆箱操作
        Console.WriteLine("--- 测试拆箱操作 ---");
        int i = (int)myArrayList[0];
        Console.WriteLine($"当前数据类型为：{i.GetType()},值：{i}");

        float f = (float)myArrayList[1];
        Console.WriteLine($"当前数据类型为：{f.GetType()},值：{f}");

        double d = (double)myArrayList[2];
        Console.WriteLine($"当前数据类型为：{d.GetType()},值：{d}");

        string str = (string)myArrayList[3];
        Console.WriteLine($"当前数据类型为：{str.GetType()},值：{str}");

        bool boolenValue = (bool)myArrayList[4];
        Console.WriteLine($"当前数据类型为：{boolenValue.GetType()},值：{boolenValue}");
    }


    /*
     * 装箱/解箱过程是相当有用的，因为它允许你假设一切都可以被当作 Object 类型来处理，而 CoreCLR 会自动帮你处理与内存有关的细节。
     * 装箱和拆箱背后的栈/堆内存转移也带来了性能问题。下面总结一下对一个简单的整型数进行装箱和拆箱所需要的步骤：
     * 1.在托管堆中分配一个新对象；
     * 2.在栈中的数据值被转移到该托管堆中的对象上；
     * 3.当拆箱时，存储在堆中对象上的值被转移回栈中；
     * 4.堆上未使用的对象将最终被 GC 回收。
     */


    /*
     * 结果：
     * 拆箱后得到System.Int32类型数据：10
     * 拆箱失败，数据原类型不是Int:Unable to cast object of type 'System.Int64' to type 'System.Int32'.
     * --- 测试ArrayList ---
     * 10
     * 17
     * 24.44
     * 333
     * True
     * --- 测试拆箱操作 ---
     * 当前数据类型为：System.Int32,值：10
     * 当前数据类型为：System.Single,值：17
     * 当前数据类型为：System.Double,值：24.44
     * 当前数据类型为：System.String,值：333
     * 当前数据类型为：System.Boolean,值：True
     */
}