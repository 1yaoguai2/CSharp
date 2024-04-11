using System.Collections;

namespace CSharpStudy;

public class ArrayListStudy
{
    public void Test()
    {
        //新增
        var _arrayList = new ArrayList();
        _arrayList.Add(1);
        _arrayList.Add("2");
        _arrayList.Add(true);
        _arrayList.Add(new object());
        Console.WriteLine("打印_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        var _arrayList1 = new ArrayList();
        _arrayList1.Add("拼接");

        //拼接
        _arrayList.AddRange(_arrayList1);

        Console.WriteLine("打印拼接后的_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        //插入元素
        _arrayList.Insert(2, "在2位置插入");


        //插入一组元素
        var _arrayList2 = new ArrayList();
        _arrayList2.Add("A");
        _arrayList2.Add("B");
        _arrayList2.Add("C");

        _arrayList.InsertRange(5, _arrayList2);

        Console.WriteLine("打印在2位置插入和在拼接处插入数组的_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        //移除
        _arrayList.RemoveAt(0);
        _arrayList.Remove("A");

        Console.WriteLine("打印移除第一个元素和移除第一个“A”元素的_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        //查找元素是否存在
        var findA = _arrayList.Contains("A");
        var findB = _arrayList.Contains("B");
        Console.WriteLine($"查找元素A是否存在：{findA}, 查找元素B是否存在：{findB}");

        //查找元素索引
        _arrayList.Add("B");
        var indexFirst = _arrayList.IndexOf("B");
        var indexLast = _arrayList.LastIndexOf("B");

        Console.WriteLine($"打印元素B的第一个索引：{indexFirst},打印元素B的最后一个索引：{indexLast}");

        //修改值,索引不存在则报错
        _arrayList[7] = "D";
        Console.WriteLine($"打印修改后_arrayList索引为7的值：{_arrayList[7]}");

        //转换出新的备份进行操作 
        var _arrayListNew = _arrayList;
        _arrayListNew.Remove(_arrayListNew.Count - 1);

        Console.WriteLine("打印备份_arrayListNew的最后一个元素被移除后的_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        var _arrayListNew2 = new ArrayList();
        _arrayListNew2 = _arrayList;
        _arrayListNew2.RemoveAt(_arrayListNew2.Count - 1);

        Console.WriteLine("打印备份_arrayListNew2的最后一个元素被移除后的_arrayLsit的值");
        foreach (var obj in _arrayList) Console.WriteLine(obj);

        //ArrayList反转
        _arrayList.Reverse();

        //ArrayList排序，可对int，float，double数据类型排序
        //存在无法比较大小的数据则报错： Failed to compare two elements in the array.
        //_arrayList.Sort();    


        //清空
        _arrayList.Clear();
        Console.WriteLine($"打印清空后的_arrayList的数组大小：{_arrayList.Count}");

        /*https://blog.csdn.net/zoujiandong_8888/article/details/115394222*/

        /* 数组，优点：内存连续，索引速度快，赋值改值简单方便，多维。缺点：声明时需要指定数组长度，插入数据麻烦。*/
        /* ArrayList，优点：大小按照存储的数据动态扩张和收缩，插入数据简单。缺点：属于Object类，存储多种类型数据，不是类型安全，存储和检索时会出现装箱和拆箱的操作，消耗性能。*/
        /* List，优点：属于ArrayList的泛型等效类，只能存储一种类型数据，避免了类型安全问题和性能消耗问题。缺点：只能存储一种类型的数据*/

        /* 装箱：将有实际类型的变量的值赋值给一个Object对象实例。
           string str1 = "abc";
           object obj1 = (object)str1;
           拆箱：将Object对象的实例的值赋给特定的变量的值。
           object obj2 = "xyz";
           string str2 = (string)obj2;
         */

        //同类型知识
        /* Stack 栈 */
        /* Queue 队列 */
        /* HashTable 哈希散列表 */
        /* Dictionary 字典 */
    }
}