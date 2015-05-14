using System;

namespace Compiler_SI.Tree
{
    class IfSt
    {
        public string Leftpart, Rightpart;
        public bool Altpart;
        public Statements ThenStatements, ElseStatements;

        public void Print(int n)
        {
            Console.WriteLine(new string(' ', n) + "Incomplete condition statement(IF):");
            Console.WriteLine(new string(' ', n+1) + "Variable identifier:");
            Console.WriteLine(new string(' ', n+2) + Leftpart);
            Console.WriteLine(new string(' ', n+1) + "Unsigned integer:");
            Console.WriteLine(new string(' ', n+2) + Rightpart);
            ThenStatements.Print(n);
            if (!Altpart) return;
            Console.WriteLine(new string(' ', n) + "Alternative part(ELSE):");
            ElseStatements.Print(n);
        }
    }
}
