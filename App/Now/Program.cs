using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Now
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine((int)new TimeSpan(DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks).TotalSeconds);
        }
    }
}
