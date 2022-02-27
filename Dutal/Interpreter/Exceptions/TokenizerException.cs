using Dutal.Interpreter.Input;

namespace Dutal.Interpreter.Exceptions
{
    class TokenizerException : InterpreterException
    {
        public TokenizerException(string message, Location location) :
            base(ExceptionOrigin.Tokenizer, message, location)
        {}
    }
}
