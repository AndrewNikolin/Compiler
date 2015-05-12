using System;

/*Exit codes: 
 1 - File with code doesn't exist
 2 - user didn't input file name
 3 - tokens table isn't full (possibly caused by not finished program) */

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

            if (file.IndexOf(".txt", StringComparison.Ordinal)==-1)
            {
                file += ".txt";
            }

            var sc = new Scanner(file);
            sc.initialize_scan();

            Console.WriteLine("Input program:");
            Console.Write(sc.CodeFile);
            Console.WriteLine();

            Console.WriteLine();
            /*Console.WriteLine("Reserved words(0):");
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
                Console.WriteLine(tableToken.Type + " " + tableToken.Index + " " +
                                  sc.GetTokenValue(tableToken));
            }
            Console.WriteLine();*/
            foreach (var word in sc.TableOnesymbol)
            {
                Console.WriteLine((int)word.ToCharArray()[0]+" " + word);
            }
            foreach (var word in sc.TableReserved)
            {
                Console.WriteLine((401 + sc.TableReserved.IndexOf(word)) + " " + word);
            }
            foreach (var word in sc.TableConstants)
            {
                Console.WriteLine((501 + sc.TableConstants.IndexOf(word)) + " " + word);
            }
            foreach (var word in sc.TableIdentifiers)
            {
                Console.WriteLine((1001+sc.TableIdentifiers.IndexOf(word)) + " "+word);
            }

            Console.WriteLine();
            if (sc.TableErrors.Count > 0)
            {
                Console.WriteLine("Errors:");
                foreach (var error in sc.TableErrors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine();
            }

            foreach (var token in sc.TableTokens)
            {
                int id;
                switch (token.Type)
                {
                    case 1:
                        id = (int) sc.GetTokenValue(token).ToCharArray()[0];
                        break;
                    case 0:
                        id = 401 + token.Index;
                        break;
                    case 3:
                        id = 501 + token.Index;
                        break;
                    case 2:
                        id = 1001 + token.Index;
                        break;
                    default:
                        id = -1;
                        break;
                }
                Console.WriteLine(id + " " + sc.GetTokenValue(token));
            }


            if (sc.TableErrors.Count > 0)
            {
                Console.WriteLine("Fix errors first");
                Console.ReadLine();
                return;
            }


            var parser = new Parser(sc);
            if (!parser.Parse())
                Console.WriteLine("Token " + sc.GetTokenValue(parser.Error) + " caused this error: " + parser.Error.Geterrortext());
            else
            {
                Console.WriteLine("Parser didn't find any errors");
                Console.WriteLine();
                Console.WriteLine("Program's Parse Tree:");
                parser.Block.Print();
            }

            Console.ReadLine();
        }
    }
}