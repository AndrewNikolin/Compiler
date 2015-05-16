using System;
using System.Collections.Generic;

namespace Compiler_SI.Tree
{
    internal class Declarations
    {
        public List<string> LabelsList;

        public Declarations()
        {
            LabelsList = new List<string>();
        }

        public void add_label(string label)
        {
            LabelsList.Add(label);
        }

        public void Print(int n)
        {
            if (LabelsList.Count <= 0) return;
            n++;
            Console.WriteLine(new string(' ', n) + "Label declarations:");
            foreach (var i in LabelsList)
            {
                Console.WriteLine(new string(' ', n + 1) + "Unsigned integer:");
                Console.WriteLine(new string(' ', n + 2) + i);
                Console.WriteLine(new string(' ', n + 1) + "Labels list:");
                n += 2;
            }
            Console.WriteLine(new string(' ', n + 1) + "Empty");
        }
    }
}