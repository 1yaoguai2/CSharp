using System;
using System.Windows.Forms;
using PluginsBase;

namespace TestDll
{
    public class TestClass : IPlugin
    {
        public Guid Guid => new Guid("896F848C-0BC2-6056-5975-6429B38431D8");
        
        public string Menu => "测试";
        
        public string Name => "Test";
        
        public void Execute()
        {
            MessageBox.Show("插件方法执行了", "友情提示！");
        }

        public void Load()
        {
            //初始化 
        }
        
        public void Dispose()
        {
            //资源释放
        }
    }
}