namespace Compiler_SI
{
    class Error : Token
    {
        private string _errorText;

        public Error(Token t, string errorText)
        {
            _errorText = errorText;
            Type = t.Type;
            Index = t.Index;
        }

        public string Geterrortext()
        {
            return _errorText;
        }
    }
}