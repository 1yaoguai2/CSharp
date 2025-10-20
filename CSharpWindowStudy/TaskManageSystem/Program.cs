// See https://aka.ms/new-console-template for more information

#region Base
using System.Data;

public enum TaskStatus
{
   ToDo,        //代办
   InProgress,  //进行中
   Completed    //已完成
}

//任务基类
public class Task
{
    public int TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreateTime { get; set; }
    public TaskStatus Status { get; set; }

    //构造函数
    public Task(int taskId,string title,string description)
    {
        TaskId = taskId;
        Title = title;
        Description = description;
        Status = TaskStatus.ToDo;
        CreateTime = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Id:{TaskId}\n 标题:{Title}\n 描述:{Description}\n 创建时间:{CreateTime:yyyy-MM-dd HH:mm:ss}\n 状态:{Status}";
    }

}

#endregion
//工作任务（继承Task）
public class WorkTask : Task
{
    //所属项目名称
    public string ProjectName { get; set; }

    public WorkTask(int taskId,string title,string description,string projectName) : base(taskId, title, description)
    {
        ProjectName = projectName;
    }

    public override string ToString()
    {
        return $"{base.ToString()}\n 项目:{ProjectName}\n 类型:工作任务\n";
    }
}

//个人任务（继承Task）
public class PersonalTask : Task
{
    public int Priority { get; set; }

    public PersonalTask(int taskId,string title,string description,int priority) : base(taskId, title, description)
    {
        Priority = priority;
    }

    public override string ToString()
    {
        return $"{base.ToString()}\n 优先级:{Priority} 类型:个人任务\n";
    }
}

public interface ITaskManager
{
    void AddTask(Task taks);
    bool RemoveTask(int taskId);
    Task GetTaskById(int taskId);
    List<Task> GetAllTasks();
    void UpdateTaskStatus(int taskId, TaskStatus newStatus);
}


//任务管理器实现类
public class TaskManager : ITaskManager
{
    private List<Task> _tasks = new List<Task>();

    //新增
    public void AddTask(Task task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task), "任务不能为null");
        if (_tasks.Exists(t => t.TaskId == task.TaskId))
            throw new InvalidOperationException($"ID为{task.TaskId}的任务已经存在");

        _tasks.Add(task);
    }

    //移除
    public bool RemoveTask(int taskId)
    {
        var taskToRemove = GetTaskById(taskId);
        if (taskToRemove == null)
            return false;
        _tasks.Remove(taskToRemove);

        return true;
    }

    //查找
    public Task GetTaskById(int taskId)
    {
        return _tasks.Find(t => t.TaskId == taskId);
    }

    //获取所有
    public List<Task> GetAllTasks()
    {
        return new List<Task>(_tasks);
    }

    //更新任务状态
    public void UpdateTaskStatus(int taskId, TaskStatus newStatus)
    {
        var task = GetTaskById(taskId);
        if (task == null)
            throw new KeyNotFoundException($"TaskId为{taskId}的任务不存在");

        task.Status = newStatus;
    }

    //保存任务到文件
    public void SaveToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var task in _tasks)
            {
                string type = task is WorkTask ? "Work" : "Personal";
                string extra = task is WorkTask
                    ? ((WorkTask)task).ProjectName
                    : ((PersonalTask)task).Priority.ToString();
                writer.WriteLine($"{type}|{task.TaskId}|{task.Title}|{task.Description}|{task.CreateTime:o}|{task.Status}|{extra}");
            }
        }
    }

    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("文件不存在", filePath);
        _tasks.Clear();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split("|");
                if (parts.Length != 7)
                {
                    Console.WriteLine($"***警告：文件内出现格式不规范的任务记录：*任务ID：{parts[1]}*");
                    continue;  //跳过格式错误的行
                }

                try
                {
                    int id = int.Parse(parts[1]);
                    string title = parts[2];
                    string description = parts[3];
                    DateTime createTime = DateTime.Parse(parts[4]);
                    TaskStatus status = (TaskStatus)Enum.Parse(typeof(TaskStatus), parts[5]);

                    if (parts[0] == "Work")
                    {
                        var workTask = new WorkTask(id, title, description, parts[6]);
                        workTask.CreateTime = createTime;
                        workTask.Status = status;
                        _tasks.Add(workTask);
                    }
                    else if (parts[0] == "Personal")
                    {
                        int priority = int.Parse(parts[6]);
                        var personalTask = new PersonalTask(id,title,description,priority);
                        personalTask.CreateTime = createTime;
                        personalTask.Status = status;
                        _tasks.Add(personalTask);
                    }
                }
                catch
                {
                    /* 忽略解析错误的行 */
                }
            }
        }
    }
}

#region 主程序
//控制台交互程序

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("程序开始");
        TaskManager manager = new TaskManager();
        bool isRunning = true;
        Console.WriteLine("--- 任务管理系统 ---");
        while (isRunning)
        {
            Console.WriteLine("\n请输入操作：");
            Console.WriteLine("1.添加工作任务");
            Console.WriteLine("2.添加个人任务");
            Console.WriteLine("3.查看所有任务");
            Console.WriteLine("4.更新指定任务");
            Console.WriteLine("5.删除指定任务");
            Console.WriteLine("6.保存任务到文件");
            Console.WriteLine("7.从文件加载任务");
            Console.WriteLine("8.退出");
            Console.Write("请输入选项：");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddWorkTask(manager);
                    break;
                case "2":
                    AddPersonalWork(manager);
                    break;
                case "3":
                    ShowAllTasks(manager);
                    break;
                case "4":
                    UpdateTaskStatus(manager);
                    break;
                case "5":
                    RemoveTask(manager);
                    break;
                case "6":
                    SaveTasks(manager);
                    break;
                case "7":
                    LoadTasks(manager);
                    break;
                case "8":
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine($"请重新输入有效选项前的值,当前值-{input}无效");
                    break;

            }
        }

    }

    public static void AddWorkTask(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入任务ID:");
            int id = int.Parse(Console.ReadLine());
            Console.Write("输入任务标题:");
            string title = Console.ReadLine();
            Console.Write("输入任务描述:");
            string desc = Console.ReadLine();
            Console.Write("输入所属项目:");
            string projectName = Console.ReadLine();

            var task = new WorkTask(id,title,desc,projectName);
            taskManager.AddTask(task);
            Console.WriteLine("工作任务添加成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"工作任务添加失败:{ex.Message}");
        }
    }

    public static void AddPersonalWork(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入任务Id:");
            int id = int.Parse(Console.ReadLine());
            Console.Write("输入任务标题:");
            string title = Console.ReadLine();
            Console.Write("输入任务描述:");
            string desc = Console.ReadLine();
            Console.Write("输入任务优先级(1-5):");
            int priority = int.Parse(Console.ReadLine());

            var task = new PersonalTask(id, title, desc, priority);
            taskManager.AddTask(task);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"个人任务添加失败:{ex.Message}");
        }
    }

    public static void ShowAllTasks(TaskManager taskManager)
    {
        var tasks = taskManager.GetAllTasks();
        if(tasks.Count == 0)
        {
            Console.WriteLine("暂无任务");
            return;
        }

        Console.WriteLine("\n--- 所有任务 ---");
        foreach(var task in tasks)
        {
            Console.WriteLine(task);
            Console.WriteLine("-----------"); 
        }
    }

    public static void UpdateTaskStatus(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入要更新的任务ID:");
            int id = int.Parse(Console.ReadLine());
            Console.WriteLine("选择新状态：0-待办 1-进行中 2-已完成");
            int statusInput = int.Parse(Console.ReadLine());
            TaskStatus newStatus = (TaskStatus)statusInput;

            taskManager.UpdateTaskStatus(id, newStatus);
            Console.WriteLine("状态更新成功");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"更新失败：{ex.Message}");
        }
    }

    public static void RemoveTask(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入要删除的任务Id:");
            int id = int.Parse(Console.ReadLine());
            bool succes = taskManager.RemoveTask(id);
            Console.WriteLine(succes ? "删除成功" : "任务不存在");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"删除失败：{ex.Message}");
        }
    }

    public static void SaveTasks(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入要保存的路径(如tasks.txt):");
            string path = Console.ReadLine();
            taskManager.SaveToFile(path);
            Console.WriteLine("保存成功");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"保存失败：{ex.Message}");
        }
    }

    public static void LoadTasks(TaskManager taskManager)
    {
        try
        {
            Console.Write("输入加载路径(如tasks.txt):");
            string path = Console.ReadLine();
            taskManager.LoadFromFile(path);
            Console.WriteLine("加载任务成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载任务失败:{ex.Message}");
        }
    }
}

#endregion

