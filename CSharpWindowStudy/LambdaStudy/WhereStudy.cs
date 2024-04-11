using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace LambdaStudy
{
    /// <summary>
    /// 由LINQ带来的扩展方法
    /// </summary>
    public class WhereStudy
    {
        public void WhereMethod()
        {
            List<Person> persons = new List<Person>
            {
                new Person { Age = 36, Name = "张三" },
                new Person { Age = 16, Name = "小美" },
                new Person { Age = 18, Name = "小帅" },
                new Person { Age = 28, Name = "小王" },
                new Person { Age = 38, Name = "李四" },
                new Person { Age = 22, Name = "靖靖" }
            };

            Console.WriteLine("打印年龄小于20的对象:");
            persons.Where(p => p.Age < 20).ToList().ForEach(c => Console.WriteLine(c.Name));
            var count = persons.Count(p => p.Age < 20);
            Console.WriteLine($"打印年龄小于20的对象的数量:{count}");
            var find = persons.Any(p => p.Name == "靖靖");
            Console.WriteLine($"打印是否存在名为靖靖的对象：{find}");
            //Single 有且只有一条数据满足条件，只能未一个，不然报错，两种写法
            var p2 = persons.Where(p => p.Name == "小帅").Single();
            var p1 = persons.Single(p => p.Name == "小帅");
            //SingleOrDefault 最多一条满足条件，0和1都可以，多余一个会报错
            //First 至少有一条，返回第一条，少于一条报错
            //FristOrDefault 返回第一条符合条件的，如果没有，则返回默认值，int为0，string为null

            //排序，对原数据无影响
            Console.WriteLine("打印以年龄大小排序后的对象列表:");
            persons.Where(p => p.Age < 50).ToList().OrderBy(c => c.Age).ToList()
                .ForEach(f => Console.WriteLine(f.Age + "," + f.Name));
            Console.WriteLine("打印以年龄大小反向排序后的对象列表:");
            persons.OrderByDescending(p => p.Age).ToList().ForEach(f => Console.WriteLine(f.Age + "," + f.Name));

            //Skip(i)跳过i条数据，Take(i)获取第i条数据
            var item1 = persons.Skip(2).Take(1);

            //Linq 扩展常用方法
            //聚合函数，Max(),Min(),Average(),Sum(),Aggregate(),Select()
            //Max(),返回最大值, Min()返回最小值
            var age1 = persons.Where(p => p.Age > 18).Max(e => e.Age);
            var age2 = persons.Where(p => p.Age < 30).Min(e => e.Age);
            //Sum(),求和，排除为空的数据
            var sum = persons.Where(p => p.Age > 18).ToList().Sum(c => c.Age);
            Console.WriteLine($"求年龄大于18的年龄总和：{sum}");
            //Average(),求平均值，空数据按默认值处理，不排除在外
            var average = persons.Average(p => p.Age);
            Console.WriteLine($"求平均年龄：{average}");
            //Aggregate(),序列自定义累加器
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var aggregate = list.Where(p => p % 2 == 0).ToList().Aggregate((o, e) => o * e);
            Console.WriteLine($"求list中偶数的累积：{aggregate}");
            //Select(),序列循环修改
            int[] numList = new int[]{25,41,30,12,14};
            int result = numList.Select((i, j) => i - j).Sum();  // (25-0) + (41-1) + (30-2) + (12-3) + (14-4) = 122
        }

        public class Person
        {
            public int Age { get; set; }
            public string Name { get; set; }
        }
    }

    /* Where语句配合Lambda语法  https://www.cnblogs.com/DylanCecil/p/15112753.html
     * 
     *
     *
     */
}