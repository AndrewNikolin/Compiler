using System;
using System.Collections.Generic;

namespace Compiler_SI.Tree
{
    internal class Declarations
    {
        private List<string> labelsList;

        public Declarations()
        {
            labelsList = new List<string>();
        }

        public void add_label(string label)
        {
            labelsList.Add(label);
        }

        public void Print(int n)
        {
            if (labelsList.Count <= 0) return;
            n++;
            Console.WriteLine(new string(' ', n) + "Label declarations:");
            foreach (var i in labelsList)
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