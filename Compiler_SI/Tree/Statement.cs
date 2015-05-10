using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler_SI.Tree
{
    class Statement
    {
        public IfSt Ifst;
        public Gotost Gotost;
        public Gotolabel Gotolabel;

        public void Print(ref int n)
        {
            if (Ifst!=null)
            {
                Ifst.Print(n);
            }
            if (Gotost!=null)
            {
                Gotost.Print(n);
            }
            if (Gotolabel!=null)
            {
                Gotolabel.Print(ref n);
            }
        }
    }
}
