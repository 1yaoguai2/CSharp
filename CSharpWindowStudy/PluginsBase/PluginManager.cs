using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PluginsBase
{
    public class PluginManager
    {
        //只读的监控文件地址
        private readonly string _pluginPath;
        //文件监控器
        private FileSystemWatcher? _watcher;
        //插件列表
        private readonly List<IPlugin> _plugins = new();

        /// <summary>
        /// 插件列表
        /// </summary>
        public List<IPlugin> Plugins => _plugins;

        //插件跟新事件
        public event Action? PluginsUpdated;
        
        public PluginManager(string pluginPath)
        {
            //设置插件文件夹
            _pluginPath = pluginPath;
            //开启文件夹监控
            StartWatching();
        }

        /// <summary>
        /// 析构函数，在对象销毁前，自带调用
        /// </summary>
        ~PluginManager()
        {
            StopWatching();
        }

        /// <summary>
        /// 监听文件目录变化
        /// </summary>
        private void StartWatching()
        {
            if (!Directory.Exists(_pluginPath))
            {
                return;
            }

            _watcher = new FileSystemWatcher(_pluginPath, "*.dll");
            _watcher.IncludeSubdirectories = true;
            _watcher.Created += Watcher_Created;
            _watcher.Deleted += Watcher_Deleted;
            _watcher.EnableRaisingEvents = true;
        }
        
        /// <summary>
        /// 关闭目录变化
        /// </summary>
        private void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.Created -= Watcher_Created;
                _watcher.Deleted -= Watcher_Deleted;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Created(Object sender, FileSystemEventArgs e)
        {
            LoadPlugin(e.FullPath);
            PluginsUpdated?.Invoke();
        }

        /// <summary>
        /// 删除卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Deleted(Object sender, FileSystemEventArgs e)
        {
            UnloadPlugin(e.FullPath);
            PluginsUpdated?.Invoke();
        }

        /// <summary>
        /// 加载所有插件
        /// </summary>
        public void LoadAllPlugins()
        {
            //文件夹是否为空
            if (!Directory.Exists(_pluginPath))
            {
                //报错
                return;
            }
            _plugins.Clear();
            
            Array.ForEach(Directory.GetFiles(_pluginPath,"*.dll", 
                SearchOption.AllDirectories),file => LoadPlugin(file));
        }

        /// <summary>
        /// 加载单个插件
        /// </summary>
        /// <param name="pluginPath"></param>
        public void LoadPlugin(string pluginPath)
        {
            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.LoadFrom(pluginPath);
            }
            catch (Exception ex)
            {
                return;
            }
            //获取所有实现了IPlugin接口的类
            var pluginTypes = pluginAssembly.GetTypes()
                .Where(type => typeof(IPlugin).IsAssignableFrom(type));
            //下面代码更宽泛，继承来的实现也算，上面的不算
            if(pluginTypes is null) return;
            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    //创建插件实列
                    var plugin = Activator.CreateInstance(pluginType) as IPlugin;
                    //执行插件特定初始化操作
                    plugin?.Load();
                    
                    //添加到插件列表
                    if(plugin != null)
                        _plugins.Add(plugin);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("创建插件失败！" + ex);
                }
            }
        }

        public void UnloadPlugins()
        {
            StopWatching();
            _plugins.ForEach(plugin => plugin.Dispose());
            _plugins.Clear();
            
            PluginsUpdated?.Invoke();
        }

        public void UnloadPlugin(string pluginPath)
        {
            Assembly? pluginAssembly;
            try
            {
                //使用Location获取dll地址，与参数比较得到内存对应程序集
                pluginAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(asm => asm.Location == pluginPath);

            }
            catch (Exception ex)
            {
                return;
            }

            //获取所有实现了Iplugin接口的类
            var pluginTypes = pluginAssembly?.GetTypes()
                .Where(type => typeof(IPlugin).IsAssignableFrom(type));
            if(pluginTypes is null) return;
            foreach (Type pluginType in pluginTypes)
            {
                foreach (var plugin in _plugins)
                {
                    if(plugin.GetType() == pluginType)
                        plugin.Dispose();
                }

                _plugins.RemoveAll(r => r.GetType() == pluginType);
            }
        }

        /// <summary>
        /// 简单日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        private void LogExceptionDetails(Exception ex, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"记录时间：{DateTime.Now:g}");
            sb.AppendLine($"错误信息：{message}");
            sb.AppendLine(ex.ToString());
            sb.AppendLine(ex.StackTrace);
            File.AppendAllText("ExceptionDetails.log",sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ExecuteFunc((Guid, string, string) cmd)
        {
            var Item = Plugins.Where(r =>
                    r.Guid == cmd.Item1 &&
                    r.Menu == cmd.Item2 &&
                    r.Name == cmd.Item3).FirstOrDefault();
            Item?.Execute();
        }

    }
}