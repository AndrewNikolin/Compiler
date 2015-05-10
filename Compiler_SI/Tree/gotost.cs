using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler_SI.Tree
{
    class Gotost
    {
        private string label;

        public Gotost(string l)
        {
            label = l;
        }

        public void Print(int n)
        {
            Console.WriteLine(new string(' ', n) + "Unsigned integer:");
            Console.WriteLine(new string(' ', n + 1) + label);
        }
    }
}