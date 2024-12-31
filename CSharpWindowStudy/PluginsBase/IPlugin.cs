using System;

namespace PluginsBase
{
    public interface IPlugin : IDisposable
    {
        Guid Guid { get; }
        string Menu { get; }
        string Name { get; }
        void Execute();
        void Load();
    }

    #region  接口方法解析
    //为什么要有Load方法
    //可以将初始化逻辑放到Load方法中
    //提供跟灵活的初始化方式，可以选择是否Load
    //允许处理异步操作或者按需加载场景

    //为什么要有Dispose方法
    //非托管资源（如文件句柄、数据库连接、网络连接等） 需要手动释放
    //关闭文件、取消订阅事件、释放定时器等。Dispose方法提供了统一的位置
    //实现IDi山坡sable 接口，可利用using语句或者手动调用 Dispose 方法
    #endregion
}