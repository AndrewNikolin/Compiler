using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Compiler_SI
{
    internal class Scanner
    {
        public List<string> TableOnesymbol; // Таблица односимвольных
        public List<string> TableDelimeters; // Таблица разделителей
        public List<string> TableReserved; // Таблица зарезервированых слов
        public List<string> TableIdentifiers; // Таблица идентификаторов
        public List<string> TableConstants; // Таблица десятичных литералов (Констант)
        public List<string> TableErrors; // Таблица ошибок
        public string CodeFile; // Код из файла
        private string _textFileName; // Имя файла
        public List<Token> TableTokens;

        public Scanner(string inputFileName)
        {
            TableOnesymbol = new List<string>(); // Список односимвольных
            TableDelimeters = new List<string>(); // Список разделителей
            TableReserved = new List<string>(); // Список зарезервированных слов
            TableIdentifiers = new List<string>(); // Список идентификаторов
            TableConstants = new List<string>(); // Список десятичных литералов (констант)
            TableErrors = new List<string>(); // Список ошибок
            TableTokens = new List<Token>(); // Список токенов

            TableOnesymbol.Add(";");
            TableOnesymbol.Add(".");
            TableOnesymbol.Add(",");
            TableOnesymbol.Add(":");
            TableOnesymbol.Add("=");
            TableOnesymbol.Sort();

            TableDelimeters.AddRange(TableOnesymbol);
            TableDelimeters.Add(" ");
            TableDelimeters.Add("\n");
            TableDelimeters.Add("\t");
            TableDelimeters.Add("\r");

            TableReserved.Add("PROGRAM");
            TableReserved.Add("BEGIN");
            TableReserved.Add("END");
            TableReserved.Add("LABEL");
            TableReserved.Add("ENDIF");
            TableReserved.Add("IF");
            TableReserved.Add("THEN");
            TableReserved.Add("ELSE");
            TableReserved.Add("GOTO");
            _textFileName = inputFileName;
        }

        public void initialize_scan() //Начало сканирования
        {
            read_file(_textFileName);
            var splitted = Splitter();
            fill_tables(splitted);
        }

        private void fill_tables(List<string> splitted) // Заполняем все таблицы
        {
            foreach (var newToken in splitted)
            {
                var token = new Token();
                if (TableReserved.IndexOf(newToken) != -1)
                {
                    token.Type = 0;
                    token.Index = TableReserved.IndexOf(newToken);
                }
                else if (TableOnesymbol.IndexOf(newToken) != -1)
                {
                    token.Type = 1;
                    token.Index = TableOnesymbol.IndexOf(newToken);
                }
                else if (IsIdentifier(newToken))
                {
                    token.Type = 2;
                    if (TableIdentifiers.IndexOf(newToken) == -1)
                        TableIdentifiers.Add(newToken);
                    token.Index = TableIdentifiers.IndexOf(newToken);
                }
                else if (TableConstants.IndexOf(newToken) != -1 || IsConstant(newToken))
                {
                    if (TableConstants.IndexOf(newToken) != -1)
                    {
                        token.Type = 3;
                        token.Index = TableConstants.IndexOf(newToken);
                    }
                    else
                    {
                        TableConstants.Add(newToken);
                        token.Type = 3;
                        token.Index = TableConstants.IndexOf(newToken);
                    }
                }
                else
                {
                    TableErrors.Add(newToken);
                    token.Type = 4;
                    token.Index = TableErrors.IndexOf(newToken);
                }

                TableTokens.Add(token);
            }
        }

        private void read_file(string textFile) //Считываем файл в переменную
        {
            try
            {
                CodeFile = File.ReadAllText(textFile);
            }
            catch (Exception)
            {
                Console.WriteLine("Such file doesn't exist");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private List<string> Splitter() // Разделяет файл на токены
        {
            var results = new List<string>();
            var insideComment = false;
            var currToken = "";

            var i = 0;
            while (i < CodeFile.Length)
            {
                if (insideComment) // Внутри коммента
                {
                    if (i + 2 < CodeFile.Length && CodeFile[i] == '*' && CodeFile[i + 1] == ')')
                        // Если он закрывается 
                    {
                        insideComment = false;
                        i++; // Чтоб снова не анализировать символ ")"
                    }
                }
                else // Не коммент
                {
                    if (TableDelimeters.IndexOf(CodeFile[i].ToString()) != -1) // Если разделитель
                    {
                        if (currToken != "") // Если перед этим была не односимвольная
                        {
                            results.Add(currToken); // Заносим предыдущий токен в таблицу
                            currToken = ""; // Чистим прошлый токен
                        }
                        if (TableOnesymbol.IndexOf(CodeFile[i].ToString()) != -1)
                            // Если разделитель еще и односимвольный токен
                            results.Add(CodeFile[i].ToString()); // Заносим в таблицу
                    }
                    else // Если не разделитель
                    {
                        if (i + 2 < CodeFile.Length && CodeFile[i] == '(' && CodeFile[i + 1] == '*')
                            // Если начинается коммент
                        {
                            insideComment = true;
                            i++; // Чтоб снова не анализировать "*"
                        }
                        else // Не разделитель и не коммент
                            currToken += CodeFile[i]; // Накапливаем токен
                    }
                }

                i++;
            }

            if (insideComment) // Незакрытый коммент
                TableErrors.Add("Comment is not closed");
            if (currToken != "")
                results.Add(currToken);

            return results;
        }


        private bool IsIdentifier(string word) //Проверяет является ли входящая строка идентификатором
        {
            if (word == "" || !char.IsLetter(word[0]))  // И IsLetter некорректно проверяет (нам нужна только латиница, а здесь пропускаются все буквы)
                return false;

            return
                word.All(
                    c =>
                        char.GetUnicodeCategory(c) == UnicodeCategory.UppercaseLetter ||
                        char.GetUnicodeCategory(c) == UnicodeCategory.DecimalDigitNumber); // Проверка на большие буквы или цифры в идентификаторе
        }

        private bool IsConstant(string word) //Проверяет является ли входящая строка десятичным литералом (константой)
        {
            return word.All(char.IsDigit);
        }

        public string GetTokenValue(Token t) //Возвращает строковое представление токена из соответствующей таблицы
        {
            switch (t.Type)
            {
                case 0:
                    return TableReserved[t.Index];
                case 1:
                    return TableOnesymbol[t.Index];
                case 2:
                    return TableIdentifiers[t.Index];
                case 3:
                    return TableConstants[t.Index];
                case 4:
                    return TableErrors[t.Index];
                default:
                    return "Unknown token";
            }
        }
    }
}