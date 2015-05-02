using System;

namespace Compiler_SI
{
    internal class Program
    {
        private static void Main()
        {
            Console.Write("Input file name: ");
            var file = Console.ReadLine();
            Console.WriteLine();

            if (file==null)
            {
                Console.WriteLine("Empty file");
                Environment.Exit(2);
            }

            if (file.IndexOf(".txt")==-1)
            {
                file += ".txt";
            }

            Scanner sc = new Scanner(file);
            sc.initialize_scan();

            Console.WriteLine("Input program:");
//            Console.WriteLine("-");
            Console.Write(sc.CodeFile);
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Reserved words(0):");
            foreach (var word in sc.TableReserved)
            {
                Console.WriteLine(sc.TableReserved.IndexOf(word) + " " + word);
            }
            Console.WriteLine();

            Console.WriteLine("One-symbols(1):");
            foreach (var symbol in sc.TableOnesymbol)
            {
                Console.WriteLine(sc.TableOnesymbol.IndexOf(symbol) + " " + symbol);
            }
            Console.WriteLine();

            Console.WriteLine("Identifiers(2):");
            foreach (var identifier in sc.TableIdentifiers)
            {
                Console.WriteLine(sc.TableIdentifiers.IndexOf(identifier) + " " + identifier);
            }
            Console.WriteLine();

            Console.WriteLine("Literals(3):");
            foreach (var constant in sc.TableConstants)
            {
                Console.WriteLine(sc.TableConstants.IndexOf(constant) + " " + constant);
            }
            Console.WriteLine();

            if (sc.TableErrors.Count > 0)
            {
                Console.WriteLine("Errors(4):");
                foreach (var error in sc.TableErrors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine();
            }


            Console.WriteLine("Table of tokens:");
            foreach (var tableToken in sc.TableTokens)
            {
                Console.WriteLine(tableToken.Type.ToString() + " " + tableToken.Index.ToString() + " " +
                                  sc.GetTokenValue(tableToken));
            }
            Console.ReadLine();
        }
    }
}