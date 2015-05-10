using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler_SI.Tree
{
    class Block
    {
        public Declarations Declarations;
        private string _programName;
        public Statements Statements;

        public Block(string name)
        {
            _programName = name;
        }

        public void Print()
        {
            Console.WriteLine("Program:");
            Console.WriteLine(' ' + _programName);
            if (Declarations != null)
            {
                Declarations.Print(0);
            }
            Statements.Print(0);
        }
    }
}