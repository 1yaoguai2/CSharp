// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


#region 1.自定义特性
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property,AllowMultiple = false)]
public class SpecialBookAttribute : Attribute
{
    public string Reason { get; }  //特殊原因
    public SpecialBookAttribute(string reason)
    {
        Reason = reason;
    }
}

//特性：验证图书ISBN格式（抽象类验证基类）
public abstract class ValidationAttribute : Attribute
{
    public abstract bool IsValid(object value);
    public abstract string ErrorMessage { get; }
}


//特性：具体实现ISBN验证（必须是13位数字）
public class IsbnValidationAttribute : ValidationAttribute
{
    public override bool IsValid(Object value)
    {
        if (value is not string isbn) return false;
        return isbn.Length == 13 && long.TryParse(isbn, out _);
    }
    public override string ErrorMessage => "ISBN必须是13位数字";
}

#endregion

#region 2.实体类（属性、特性）
//图书类别枚举
public enum BookCategory
{
    Fiction,     //小说
    Science,     //科学
    Technology,  //技术
    History,     //历史
    Other        //其他
}

[SpecialBook("无特殊标记")]
public class Book
{
    //属性：ID（只读）
    public int Id { get; }

    [IsbnValidation]
    public string Isbn { get; set; }

    //属性：标题（私有字段+共有属性）
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("标题不能为空");
            _title = value;
        }
    }

    public string Author { get; set; } = string.Empty;
    public BookCategory Category { get; set; }

    public DateTime PublishDate { get; set; }

    public bool IsAvailable => BorrowerId == -1;

    private int _borrowerId = -1;
    public int BorrowerId
    {
        get => _borrowerId;
        internal set => _borrowerId = value;
    }

    public Book(int id, string isbn, string title, string author, BookCategory category, DateTime publishDate)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        Author = author;
        Category = category;
        PublishDate = publishDate;
    }

    public override string ToString()
    {
        return $"ID: {Id}, ISBN: {Isbn}, 标题：{Title}, 作者：{Author}, 类别：{Category}, 状态：{(!IsAvailable ? $"已被ID为{BorrowerId}的读者借出":"可借阅")}";
    }
}

[SpecialBook("珍贵藏书")]
public class SpecialBook : Book
{
    public SpecialBook(int id, string isbn, string title, string author, BookCategory category, DateTime publishDate)
        : base(id, isbn, title, author, category, publishDate)
    {
    }
}

public class Reader
{
    public int Id { get; }
    public string ReaderName { get; set; } = string.Empty;
    public int Age { get; set; }

    public Reader(int id, string readerName, int age)
    {
        Id = id;
        ReaderName = readerName;
        Age = age;
    }
}
#endregion

#region 3.泛型仓库（泛型、集合知识点）
//泛型数据仓库接口
public interface IRepository<T> where T : class
{
    void Add(T item);
    bool Remove(T item);
    IEnumerable<T> GetAll();
    T GetById(int id);
}

//仓库泛型
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly List<T> _items = new List<T>();

    public virtual void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    public virtual bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _items.ToList();
    }

    public virtual T GetById(int id)
    {
        PropertyInfo idProp = typeof(T).GetProperty("Id");
        if (idProp == null) throw new InvalidOperationException($"{typeof(T)}没有Id属性");
        return _items.FirstOrDefault(item => (int)idProp.GetValue(item) == id);
    }
}
#endregion

#region 4.图书管理器（索引器、委托、事件知识点）
//委托：图书状态变化时的通知委托
public delegate void BookStatusChangedHandler(Book book, string message);

public class BookManager : Repository<Book>
{
    //事件：图书被借出时触发
    public event BookStatusChangedHandler BookBorrowed;
    //事件：图书归还时触发
    public event BookStatusChangedHandler BookReturned;

    //索引器：通过ID或ISBN访问图书管（索引器知识点）
    public Book this[string key]
    {
        get
        {
            if(int.TryParse(key, out int id))
                return GetById(id);
            return _items.FirstOrDefault(b => b.Isbn == key);
        }
    }

    public Book this[int index]
    {
        get => _items[index];
    }

    public override void Add(Book book)
    {
        //反射验证所有带ValidationAttribute的属性
        var properties = typeof(Book).GetProperties().Where(p => p.GetCustomAttributes<ValidationAttribute>().Any());

        foreach(var prop in properties)
        {
            var validationAttr = prop.GetCustomAttribute<ValidationAttribute>();
            object value = prop.GetValue(book);
            if (!validationAttr.IsValid(value)) throw new ArgumentException($"{prop.Name}验证失败：{validationAttr.ErrorMessage}");
        }

        base.Add(book);
    }

    //借阅图书
    public bool BorrowBook(int bookId,int readerId)
    {
        Book book = GetById(bookId);
        if (book is null) return false;
        if (!book.IsAvailable) return false;

        book.BorrowerId = readerId;
        //触发借出事件
        BookBorrowed?.Invoke(book, $"图书《{book.Title}》借阅者ID:{readerId}借出");
        return true;
    }

    //归还图书
    public bool ReturnBook(int bookId)
    {
        Book book = GetById(bookId);
        if (book == null) return false;
        if (book.IsAvailable) return false;

        int borrowerId = book.BorrowerId;
        book.BorrowerId = -1;

        //触发规划事件
        BookReturned?.Invoke(book, $"图书《{book.Title}》由读者Id：{borrowerId}归还");
        return true;
    }

    //Linq查询：多条件筛选图书
    public IEnumerable<Book> SerchBooks(string titleKeyword = null, string autor = null, BookCategory? category = null, bool? isAvailable = null)
    {
        IEnumerable<Book> query = _items;

        if (!string.IsNullOrEmpty(titleKeyword))
            query = query.Where(b => b.Title.Contains(titleKeyword, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(autor))
            query = query.Where(b => b.Author == autor);

        if (category.HasValue)
            query = query.Where(b => b.Category == category.Value);

        if (isAvailable.HasValue)
            query = query.Where(b => b.IsAvailable == isAvailable.Value);

        //按出版日期倒序
        return query.OrderByDescending(b => b.PublishDate).ToList();
    }

    //反射获取特殊图书（反射+特性知识点）
    public IEnumerable<Book> GetSpecialBooks()
    {
        foreach(var book in _items)
        {
            //所有Book类都是无特殊标记，所以无法找到SpecialBook的图书
            //var attr = typeof(Book).GetCustomAttribute<SpecialBookAttribute>();
            //优化，根据每个实例进行反射检查
            var bookType = book.GetType();
            var attr = bookType.GetCustomAttribute<SpecialBookAttribute>();

            if (attr != null && attr.Reason != "无特殊标记")
                yield return book;
        }
    }
}

#endregion

#region 5.主程序（交互演示）
class Program
{
    static void Main(string[] args)
    {
        try
        {
            //初始化图书管理器
            var bookManager = new BookManager();
            //实例化一个读者数据仓库
            var readerReop = new Repository<Reader>();

            //注册事件处理（委托＋匿名函数）
            bookManager.BookBorrowed += (book, msg) => Console.WriteLine($"[系统通知]：{msg}");
            bookManager.BookReturned += (book, msg) => Console.WriteLine($"[系统通知]:{msg}");

            //添加测试数据
            AddTestData(bookManager, readerReop);

            Console.WriteLine($"=== 智能图书管理系统 ===");
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n请选择操作：");
                Console.WriteLine("1. 查看所有图书");
                Console.WriteLine("2. 按条件查询图书");
                Console.WriteLine("3. 借阅图书");
                Console.WriteLine("4. 归还图书");
                Console.WriteLine("5. 查看特殊图书（反射特性演示）");
                Console.WriteLine("6. 退出");
                Console.Write("输入选项：");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowAllBooks(bookManager);
                        break;
                    case "2":
                        SearchBooks(bookManager);
                        break;
                    case "3":
                        BorrowBook(bookManager);
                        break;
                    case "4":
                        ReturnBook(bookManager);
                        break;
                    case "5":
                        ShowSpecialBooks(bookManager);
                        break;
                    case "6":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine($"无效选项{input}");
                        break;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"图书管理系统报警：{ex.Message}");
            Console.Write("输入任意键退出系统");
            Console.ReadKey();
        }
    }

    static void AddTestData(BookManager bookManager, IRepository<Reader> readerPepo)
    {
        try
        {
            bookManager.Add(new Book(1, "9787115546081", "C#编程指南", "微软", BookCategory.Technology, new DateTime(2020, 1, 1)));
            bookManager.Add(new Book(2, "9787115546082", "深入理解C#", "JonSkeet", BookCategory.Technology, new DateTime(2018, 5, 1)));
            bookManager.Add(new Book(3, "9787115546083", "人类简史", "尤瓦尔.赫拉利", BookCategory.History, new DateTime(2014, 11, 1)));
            //ISBN无效，会触发验证失败
            //bookManager.Add(new Book(4, "invalid", "测试图片", "测试作者", BookCategory.Other, DateTime.Now));

            bookManager.Add(new SpecialBook(5, "9787115546085", "大象", "李象", BookCategory.Technology, new DateTime(2017, 12, 19)));
        }
        catch(Exception ex)
        {
            Console.WriteLine($"添加测试数据失败{ex.Message}");
            throw;
        }

        //添加读者
        readerPepo.Add(new Reader(101, "张三", 25));
        readerPepo.Add(new Reader(102, "李四", 30));
    }

    static void ShowAllBooks(BookManager manager)
    {
        Console.WriteLine("\n--- 所有图书 ---");
        foreach(var book in manager.GetAll())
        {
            Console.WriteLine(book);
        }
    }

    static void SearchBooks(BookManager manager)
    {
        Console.Write("输入标题关键词（可选）：");
        string title = Console.ReadLine();
        Console.Write("输入作者（可选）：");
        string author = Console.ReadLine();
        Console.Write("输入类别（0-小说 1—科技 2-技术 3-历史 4-其他，可选）：");
        //未输入值，导致cat为0，让所有类型都为小说
        //int.TryParse(Console.ReadLine(),out int cat);
        //BookCategory? category = cat >= 0 && cat <= 4 ? (BookCategory)cat : null;
        //优化
        BookCategory? category = null;
        var categoryId = Console.ReadLine();
        if(!string.IsNullOrEmpty(categoryId))
            category = (BookCategory)int.Parse(categoryId);
        Console.Write("是否可借阅（y/n,可选）：");
        string avail = Console.ReadLine();
        bool? isAvailable = avail.ToLower() == "y" ? true : (avail.ToLower() == "n" ? false : null);

        var results = manager.SerchBooks(title, author, category, isAvailable);

        Console.WriteLine("\n--- 查询结果 ---");
        foreach (var book in results)
        {
            Console.WriteLine(book);
        }
    }

    static void BorrowBook(BookManager manager)
    {
        Console.Write("输入图书ID：");
        int.TryParse(Console.ReadLine(), out int bookId);
        Console.Write("输入读者ID：");
        int.TryParse(Console.ReadLine(), out int readerId);

        bool success = manager.BorrowBook(bookId, readerId);
        Console.WriteLine(success ? "借阅成功" : "借阅失败（图书不存在或已借出）");
    }

    static void ReturnBook(BookManager manager)
    {
        Console.Write("输入图书ID：");
        int.TryParse(Console.ReadLine(), out int bookId);
        bool success = manager.ReturnBook(bookId);
        Console.WriteLine(success ? "归还成功" : "归还失败（图书不存在或未借出）");
    }

    static void ShowSpecialBooks(BookManager manager)
    {
        Console.WriteLine("\n--- 特殊图书 ---");
        var specialBooks = manager.GetSpecialBooks();
        if (!specialBooks.Any())
        {
            Console.WriteLine("暂无特殊图书");
            return;
        }

        foreach(var book in specialBooks)
        {
            var bookType = book.GetType();
            var attr = bookType.GetCustomAttribute<SpecialBookAttribute>();

            Console.WriteLine($"类型：{attr?.Reason} {book}");
        }
    }
}
#endregion