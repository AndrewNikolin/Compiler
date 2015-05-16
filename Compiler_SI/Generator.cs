using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler_SI.Tree;

namespace Compiler_SI
{
    class Generator
    {
        private Block _programTree; // Дерево программы
        private string _filename; // Имя файла
        private List<string> _labels; // Объявленные метки
        private List<string> _asmlines; // Строки сгенерированного кода
        private List<string> _gotolabels; // Метки использованные в GOTO
        private List<string> _labelsList; // Метки расположенные в коде
        private int _systemlabels; // Номер внутренней метки ?L#

        public Generator(Block programTree, string filename)
        {
            _programTree = programTree;
            _filename = filename;
            if (programTree.Declarations!=null)
            {
                _labels = programTree.Declarations.LabelsList;
            }
            _asmlines = new List<string>();
            _labelsList = new List<string>();
            _gotolabels = new List<string>();
            _systemlabels = 0;
        }

        public string Generatecode()
        {
            _asmlines.Add("TITLE " + _programTree._programName);
            _asmlines.Add("start_point:");
            StatementsGenerator(_programTree.Statements);
            _asmlines.Add("END start_point");
            foreach (var gotolabel in _gotolabels.Where(gotolabel => _labelsList.IndexOf(gotolabel)==-1))
            {
                return "Label " + gotolabel + " is used in GOTO, but not placed in code";
            }
            foreach (var label in _labelsList.Where(label => _labels.IndexOf(label)==-1))
            {
                return "Label " + label + " wasn't declared";
            }

            System.IO.File.WriteAllLines(_filename + ".asm", _asmlines);
           
            return "Code successfully generated";
        }

        private void StatementsGenerator(Statements statements)
        {
            foreach (var statement in statements.St)
            {
                if (statement.Ifst!=null)
                {
                    IfGenerator(statement.Ifst);
                }
                else if (statement.Gotolabel != null)
                {
                    _labelsList.Add(statement.Gotolabel.label);
                    _asmlines.Add("?U" + statement.Gotolabel.label + ":");
                }
                else if(statement.Gotost!=null)
                {
                    _gotolabels.Add(statement.Gotost.label);
                    _asmlines.Add("JMP ?U" + statement.Gotost.label);
                }
            }
        }

        private void IfGenerator(IfSt ifSt)
        {
            if (ifSt.Leftpart == _programTree._programName)
            {
                Console.WriteLine("You can't use program name as variable");
                Console.ReadLine();
                System.Environment.Exit(4);
            }
            _asmlines.Add("MOV AX, " + ifSt.Leftpart);
            _asmlines.Add("MOV BX, " + ifSt.Rightpart);
            _asmlines.Add("CMP AX, BX");
            if (!ifSt.Altpart)
            {
                var savedLabel = _systemlabels;
                _systemlabels++;
                _asmlines.Add("JNE ?L" + savedLabel);
                StatementsGenerator(ifSt.ThenStatements);
                _asmlines.Add("?L" + savedLabel + ":");
            }
            else
            {
                var savedPos = _asmlines.Count;
                StatementsGenerator(ifSt.ThenStatements);
                _asmlines.Insert(savedPos, "JNE ?L" + _systemlabels);
                savedPos = _asmlines.Count;
                _asmlines.Add("?L" + _systemlabels + ":");
                _systemlabels++;
                StatementsGenerator(ifSt.ElseStatements);
                _asmlines.Insert(savedPos, "JMP ?L" + _systemlabels);
                _asmlines.Add("?L" + _systemlabels + ":");
                _systemlabels++;
            }
        }

    }
}