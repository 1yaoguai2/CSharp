using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingStudy
{
    //进程是一个执行中的程序的抽象，包含代码、数据、堆栈、资源等 ；
    //线程是进程的一个执行单元，进程可包含多个线程，之间共享资源，但拥有独立的执行流程；
    //多线程适用于需要提升系统并发性、数据吞吐量、响应速度等常见，充分利用多核处理器和系统资源，提高应用程序的性能和效率。
    //常用于CPU或I/O密集型任务、并发请求处理、大数据处理。
    public static class ThreadStart
    {

        //常见的几种启动多线程的方式

        //Thread类
        public static void ThreadMethod()
        {
            Thread thread = new Thread(WorkMethod);
            thread.Start();


            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId}，正在执行任务{i}");
                Thread.Sleep(100);
            }
        }

        private static void WorkMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId},正在执行任务{i}");
                Thread.Sleep(100);
            }
        }


        //ThreadPool
        public static void ThreadPoolMethod()
        {
            ThreadPool.QueueUserWorkItem(o => WorkMethod());
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId}，正在执行任务{i}");
                Thread.Sleep(100);
            }
        }


        //Parallel -平行
        public static void ParallelMethod()
        {
            Parallel.Invoke(WorkMethod,WorkMethod1,WorkMethod2);
        } 

        private static void WorkMethod1()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId},正在执行任务{i}");
                Thread.Sleep(100);
            }
        }

        private static void WorkMethod2()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId},正在执行任务{i}");
                Thread.Sleep(100);
            }
        }
    }
}
