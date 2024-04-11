using System;

namespace LambdaStudy
{
    public class StatusMethodClass
    {
        private string log = "";

        public void StatusMethod(string? info = null)
        {
            log = info ?? log;
            Console.WriteLine(log);
        }
    }
}