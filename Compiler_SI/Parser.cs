using System;

namespace Compiler_SI
{
    internal class Parser
    {
        private int _tokenId; // Порядковый номер(база 0) токена в таблице токенов
        public Error Error;
        private Scanner sc;

        public Parser(Scanner sc) // Конструктор, получает параметр типа Scanner
        {
            this.sc = sc;
            _tokenId = -1;
        }

        private Token GetnextToken() // Получить следующий токен из таблицы
        {
            _tokenId++;
            if (_tokenId >= sc.TableTokens.Count)
                critical_error();
            return sc.TableTokens[_tokenId];
        }

        public bool Parse() // Начало грамматического анализа
        {
            return Program();
        }

        private bool Program() // Проверка PROGRAM <identifier>;
        {
            var token = GetnextToken();
            if (sc.GetTokenValue(token) != "PROGRAM")
            {
                add_error(token, "PROGRAM expected");
                return false;
            }

            if (Identifier()) return Commandend() && Block();
            add_error(token, "Identifier expected");
            return false;
        }

        private bool Identifier() // Проверяет является ли токен идентификатором
        {
            var token = GetnextToken();
            if (sc.TableIdentifiers.IndexOf(sc.GetTokenValue(token)) != -1) return true;
            add_error(token, "Identifier expected");
            return false;
        }

        private bool Block() // Проверяет тело программы <labels> BEGIN <statements> END
        {
            var savedPos = _tokenId;
            if (Labels() == false && Error == null)
            {
                _tokenId = savedPos;
            }
            else
            {
                if (Error != null)
                {
                    return false;
                }
            }

            var token = GetnextToken();
            if (sc.GetTokenValue(token) != "BEGIN")
            {
                add_error(token, "BEGIN expected");
                return false;
            }
            switch (Statements())
            {
                case true:
                    _tokenId--;
                    token = GetnextToken();
                    if (sc.GetTokenValue(token) != "END")
                    {
                        add_error(token, "END expected");
                        return false;
                    }
                    if (sc.TableTokens.Count - 1 <= _tokenId) return true;
                    add_error(token, "Additional symbols after END must be removed");
                    return false;
                case false:
                    return false;
            }
            return false;
        }

        private bool Statements() // Проверяет список операторов
        {
            var token = GetnextToken();
            switch (sc.GetTokenValue(token))
            {
                case "END":
                    return true;
                case "ELSE":
                    return true;
                case "ENDIF":
                    return true;
                case ";":
                    return Statements();
            }

            if (sc.TableConstants.IndexOf(sc.GetTokenValue(token)) != -1)
            {
                token = GetnextToken();
                if (sc.GetTokenValue(token) == ":") return Statements();
                add_error(token, "You must use ':' after label name");
                return false;
            }

            if (sc.GetTokenValue(token) == "GOTO")
            {
                token = GetnextToken();
                if (sc.TableConstants.IndexOf(sc.GetTokenValue(token)) == -1)
                {
                    add_error(token, "You must use label name(unsigned integer) after GOTO statement");
                }
                else
                {
                    token = GetnextToken();
                    if (sc.GetTokenValue(token) == ";") return Statements();
                    add_error(token, "You must use ';' after statement end");
                    return false;
                }
            }

            if (sc.GetTokenValue(token) == "IF")
            {
                return ConditionStatement();
            }
            add_error(token, "Mistake in program code");
            return false;
        }

        private bool ConditionStatement() // Проверяет условное выражение IF <condition> THEN <statements> ELSE <statements> ENDIF;
        {
            var token = GetnextToken();
            if (sc.TableIdentifiers.IndexOf(sc.GetTokenValue(token)) == -1)
            {
                add_error(token, "You must use identifier at the left part of conditional expression");
                return false;
            }
            token = GetnextToken();
            if (sc.GetTokenValue(token) != "=")
            {
                add_error(token, "Only equal comparison is allowed in conditional expressions");
                return false;
            }
            token = GetnextToken();
            if (sc.TableConstants.IndexOf(sc.GetTokenValue(token)) == -1)
            {
                add_error(token, "You must use unsigned integer at the right part of conditional expression");
                return false;
            }
            token = GetnextToken();
            if (sc.GetTokenValue(token) != "THEN")
            {
                add_error(token, "You must use 'THEN' at the end of conditional expression");
                return false;
            }
            if (!Statements()) return false;
            _tokenId--;
            token = GetnextToken();
            switch (sc.GetTokenValue(token))
            {
                case "ELSE":
                    if (Statements())
                    {
                        _tokenId--;
                        token = GetnextToken();
                        if (sc.GetTokenValue(token) != "ENDIF") return true;
                        token = GetnextToken();
                        if (sc.GetTokenValue(token) == ";") return Statements();
                        add_error(token, "You must use ';' after statement end");
                        return false;
                    }
                    break;
                case "ENDIF":
                    token = GetnextToken();
                    if (sc.GetTokenValue(token) != ";")
                    {
                        add_error(token, "You must use ';' after statement end");
                        return false;
                    }
                    return Statements();
                default:
                    add_error(token, "You've made mistake in condition statement in ELSE or ENDIF part");
                    return false;
            }
            return false;
        }

        private bool Labels() // Проверяет список меток
        {
            var token = GetnextToken();
            return sc.GetTokenValue(token) == "LABEL" && LabelsList();
        }

        private bool LabelsList()
        {
            var token = GetnextToken();
            if (sc.TableConstants.IndexOf(sc.GetTokenValue(token)) == -1)
            {
                add_error(token, "Decimal literal expected");
                return false;
            }
            token = GetnextToken();
            switch (sc.GetTokenValue(token))
            {
                case ",":
                    return LabelsList();
                case ";":
                    return true;
                default:
                    add_error(token, "Wrong labels list");
                    return false;
            }
        }

        private bool Commandend() // Проверяет окончание команды(наличие ";")
        {
            var token = GetnextToken();
            if (sc.GetTokenValue(token) == ";") return true;
            add_error(token, "';' expected");
            return false;
        }

        private void add_error(Token tCaused, string errorText)
            // Добавляет ошибку в список. Параметры: токен вызвавший ошибку, текст ошибки
        {
            var nError = new Error(tCaused, errorText);
            Error = nError;
        }

        private void critical_error() // Ошибка вызывающаяся в случае окончания списка токенов
        {
            Console.WriteLine("Error: you have to complete your code");
            Console.ReadLine();
            Environment.Exit(3);
        }
    }
}