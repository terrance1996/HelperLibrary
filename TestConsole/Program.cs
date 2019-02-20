using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperLibrary;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
           Post.locResult locResult = Post.locResult.checkLocStPc("WILSON", "WA","6107");
            
            Console.WriteLine(Test.temp);

            Console.ReadLine();
        }
    }
}
