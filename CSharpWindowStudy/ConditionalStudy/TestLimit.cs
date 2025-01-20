using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConditionalStudy
{
    public class TestLimit
    {

        public void TestLimitMonthed()
        {
#if 珍藏版
            Console.WriteLine("珍藏版测试股范围");
#else
            Console.WriteLine("普通版测试范围");
#endif

        }

    }
}
