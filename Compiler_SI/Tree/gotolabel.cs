using System;

namespace Compiler_SI.Tree
{
    class Gotolabel
    {
        public string label;

        public Gotolabel(string l)
        {
            label = l;
        }

        public void Print(ref int n)
        {
            Console.WriteLine(new string(' ', n) + "Unsigned integer(GOTO Label):");
            Console.WriteLine(new string(' ', n + 1) + label);
        }
    }
}
