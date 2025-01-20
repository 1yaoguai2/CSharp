#define FREE_GAME
#undef DEBUG
#undef FREE_GAME 
#define 珍藏版
//#undef 珍藏版

// See https://aka.ms/new-console-template for more information
using ConditionalStudy;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
#if DEBUG
Console.WriteLine("游戏测试语句");
#elif FREE_GAME
Console.WriteLine("免费游戏脚本");
#else
Console.WriteLine("付费游戏脚本");
Test test = new Test();
test.GamePlay();
test.Play();

//预处理影响范围测试
TestLimit testLimit = new TestLimit();
testLimit.TestLimitMonthed();
#endif



class Test
{
    public void GamePlay()
    {
        Console.WriteLine("游戏开始！");
    }

    [Conditional("珍藏版")]
    public void Play()
    {
        Console.WriteLine("特殊玩家珍藏版！");
    }
}

/*
#define 符号名称
#undef 符号名称
-符号名称通常使用英文大写
-必须写在程序文件的顶部
-符号只能在该文件中使用，全局符号需要

使用特性进行条件编译
[Conditional("符号名称")]
 */
