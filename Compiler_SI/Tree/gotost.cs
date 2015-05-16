using System;

namespace Compiler_SI.Tree
{
    class Gotost
    {
        public string label;

        public Gotost(string l)
        {
            label = l;
        }

        public void Print(int n)
        {
            Console.WriteLine(new string(' ', n) + "Unsigned integer(GOTO):");
            Console.WriteLine(new string(' ', n + 1) + label);
        }
    }
}