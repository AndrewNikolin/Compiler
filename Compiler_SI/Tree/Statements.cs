﻿using System;
using System.Collections.Generic;

namespace Compiler_SI.Tree
{
    class Statements
    {
        public List<Statement> St;

        public Statements()
        {
            St = new List<Statement>();
        }

        public void Addst(Statement statement)
        {
            St.Add(statement);
        }

        public void Print(int n)
        {
            n++;
            Console.WriteLine(new string(' ', n) + "Statements List:");
            foreach (var statement in St)
            {
                Console.WriteLine(new string(' ', ++n) + "Statement:");
                n++;
                statement.Print(ref n);
                Console.WriteLine(new string(' ', n) + "Statements list:");
            }
            Console.WriteLine(new string(' ', n+1) + "Empty");
        }
    }
}