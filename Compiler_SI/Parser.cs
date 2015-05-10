using System;
using Compiler_SI.Tree;

namespace Compiler_SI
{
    internal class Parser
    {
        private int _tokenId; // Порядковый номер(база 0) токена в таблице токенов
        public Error Error;
        private Scanner sc;
        public Block Block;

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

            if (Identifier())
            {
                _tokenId--;
                Block = new Block(sc.GetTokenValue(GetnextToken()));
                return Commandend() && ProgramBlock();
            }
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

        private bool ProgramBlock() // Проверяет тело программы <labels> BEGIN <statements> END
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
            Block.Statements = new Statements();
            switch (StatementsCheck(ref Block.Statements))
            {
                case true:
                    _tokenId--;
                    token = GetnextToken();
                    if (sc.GetTokenValue(token) != "END")
                    {
                        add_error(token, "END expected");
                        return false;
                    }
                    else
                    {
                        token = GetnextToken();
                        if (sc.GetTokenValue(token)!=".")
                        {
                            add_error(token, "You must use '.' after END statement");
                            return false;
                        }
                    }
                    if (sc.TableTokens.Count - 1 <= _tokenId) return true;
                    add_error(token, "Additional symbols after END. must be removed");
                    return false;
                case false:
                    return false;
            }
            return false;
        }

        private bool StatementsCheck(ref Statements st) // Проверяет список операторов
        {
            var token = GetnextToken();
            switch (sc.GetTokenValue(token))
            {
                case "ELSE":
                    return true;
                case "ENDIF":
                    return true;
                case "END":
                    return true;
                case ";":
                    return StatementsCheck(ref st);
            }

            if (sc.TableConstants.IndexOf(sc.GetTokenValue(token)) != -1)
            {
                var newGotolabel = new Gotolabel(sc.GetTokenValue(token));
                var newSt = new Statement {Gotolabel = newGotolabel};
                st.Addst(newSt);
                token = GetnextToken();
                if (sc.GetTokenValue(token) == ":") return StatementsCheck(ref st);
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
                    var gotost = new Gotost(sc.GetTokenValue(token));
                    var statement = new Statement {Gotost = gotost};
                    st.Addst(statement);

                    token = GetnextToken();
                    if (sc.GetTokenValue(token) == ";") return StatementsCheck(ref st);
                    add_error(token, "You must use ';' after statement end");
                    return false;
                }
            }

            if (sc.GetTokenValue(token) == "IF")
            {
                return ConditionStatement(ref st);
            }
            add_error(token, "Mistake in program code");
            return false;
        }

        private bool ConditionStatement(ref Statements st) // Проверяет условное выражение IF <condition> THEN <statements> ELSE <statements> ENDIF;
        {
            var token = GetnextToken();
            if (sc.TableIdentifiers.IndexOf(sc.GetTokenValue(token)) == -1)
            {
                add_error(token, "You must use identifier at the left part of conditional expression");
                return false;
            }
            var ifSt = new IfSt {Leftpart = sc.GetTokenValue(token)};
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
            ifSt.Rightpart = sc.GetTokenValue(token);
            token = GetnextToken();
            if (sc.GetTokenValue(token) != "THEN")
            {
                add_error(token, "You must use 'THEN' at the end of conditional expression");
                return false;
            }
            ifSt.ThenStatements = new Statements();
            if (!StatementsCheck(ref ifSt.ThenStatements)) return false;
            _tokenId--;
            token = GetnextToken();
            switch (sc.GetTokenValue(token))
            {
                case "ELSE":
                    ifSt.ElseStatements = new Statements();
                    ifSt.Altpart = true;
                    if (StatementsCheck(ref ifSt.ElseStatements))
                    {
                        _tokenId--;
                        token = GetnextToken();
                        if (sc.GetTokenValue(token) == "ENDIF")
                        {
                            token = GetnextToken();
                            if (sc.GetTokenValue(token) != ";")
                            {
                                add_error(token, "You must use ';' after statement end");
                                return false;
                            }
                            var newSt = new Statement { Ifst = ifSt };
                            st.Addst(newSt);
                            return StatementsCheck(ref st);
                        }
                        add_error(token, "Condition statement must end with ENDIF");
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
                    var newStatement = new Statement {Ifst = ifSt};
                    st.Addst(newStatement);
                    return StatementsCheck(ref st);
                default:
                    add_error(token, "You've made mistake in condition statement in ELSE or ENDIF part");
                    return false;
            }
            return false;
        }

        private bool Labels() // Проверяет список меток
        {
            var token = GetnextToken();
            Block.Declarations = new Declarations();
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
            Block.Declarations.add_label(sc.GetTokenValue(token));
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