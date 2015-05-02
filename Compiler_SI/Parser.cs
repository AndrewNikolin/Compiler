using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler_SI
{
    internal class Parser
    {
        private int _tokenId;
        private Scanner sc;

        public Parser(Scanner sc)
        {
            this.sc = sc;
            _tokenId = -1;
        }

        private Token GetnextToken()
        {
            _tokenId++;
            return sc.TableTokens[_tokenId];
        }

        public void Parse()
        {
        }
    }
}