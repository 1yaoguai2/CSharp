using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PluginsBase;

namespace ReChaBaCeShi
{
    public partial class Form1 : Form
    {
        private readonly PluginManager _pluginManager;
        public Form1()
        {
            InitializeComponent();
            //实列化插件管理器，设置插件文件夹
            _pluginManager = new PluginManager(Path.Combine(Application.StartupPath, "Plugins"));
            //实时更新插件
            _pluginManager.PluginsUpdated += UpdatePluginList;

            CmdClick += _pluginManager.ExecuteFunc;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            _pluginManager.LoadAllPlugins();
            UpdatePluginList();
        }

        protected override void OnClosed(EventArgs e)
        {
            _pluginManager.UnloadPlugins();
            base.OnClosed(e);
        }

        private void UpdatePluginList()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(UpdatePluginList);
                    return;
                }

                pluginsMenu.DropDownItems.Clear();

                Dictionary<string, Guid> menuGuids = new();

                foreach (var plugin in _pluginManager.Plugins)
                {
                    if (!string.IsNullOrEmpty(plugin.Menu) &&
                        !string.IsNullOrEmpty(plugin.Name)
                        && plugin.Guid != default(Guid))
                    {
                        var menuItemName = plugin.Menu;
                        if (menuGuids.ContainsKey(menuItemName)
                            && menuGuids[menuItemName] == plugin.Guid)
                        {
                            var existingMenuItem = pluginsMenu.DropDownItems
                                .OfType<ToolStripMenuItem>()
                                .FirstOrDefault(menuItem => menuItem.Text.Equals(menuItemName));
                            if (existingMenuItem != null)
                            {
                                existingMenuItem.DropDownItems.Add(CreatePluginMenuItem(plugin));
                            }
                        }
                        else
                        {
                            //创建一个新的菜单选项并添加到plugins菜单中
                            var newFirstLevelMenuItem = new ToolStripMenuItem(plugin.Menu);
                            var newSecondLevelMenuItem = CreatePluginMenuItem(plugin);
                            newFirstLevelMenuItem.DropDownItems.Add(newSecondLevelMenuItem);
                            pluginsMenu.DropDownItems.Add(newFirstLevelMenuItem);
                                
                            //保存菜单项名称和ID的映射关系
                            menuGuids[menuItemName] = plugin.Guid;
                        }
                    }
                }

            }
            catch (InvalidCastException)
            {
                UpdatePluginList();
                return;
            }
            catch
            {
                //异常处理逻辑
                Console.WriteLine("加载插件出错");
            }
        }

        private event Action<(Guid, string, string)> CmdClick;

        private ToolStripMenuItem CreatePluginMenuItem(IPlugin plugin)
        {
            var menuItem = new ToolStripMenuItem(plugin.Name);
            menuItem.Click += (sender, e) =>
            {
                CmdClick?.Invoke((plugin.Guid, plugin.Menu, plugin.Name));
            };
            return menuItem;
        }

    }
}