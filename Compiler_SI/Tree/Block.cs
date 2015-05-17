using System;

namespace Compiler_SI.Tree
{
    class Block
    {
        public Declarations Declarations;
        public string _programName;
        public Statements Statements;

        public Block(string name)
        {
            _programName = name;
        }

        public void Print()
        {
            Console.WriteLine("Program:");
            Console.WriteLine(" Identifier:");
            Console.WriteLine("  " + _programName);
            Console.WriteLine(" Block:");
            if (Declarations != null)
            {
                Declarations.Print(1);
            }
            Statements.Print(1);
        }
    }
}