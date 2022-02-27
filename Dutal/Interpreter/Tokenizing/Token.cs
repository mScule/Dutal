namespace Dutal.Interpreter.Tokenizing
{
    enum TokenType
    {
        Symbol,
        String,
        Word,
        Interface,
        UserDefined,
        Number,
        EndOfFile
    }

    struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() =>
            $"[\"{Value}\" : {Type}]";

        public bool Equals(Token token) =>
            Type.Equals(token.Type) && Value.Equals(token.Value);
    }
}
