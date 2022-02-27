using Dutal.Interpreter.Input;

namespace Dutal.Interpreter.Exceptions
{
    class ParserException : InterpreterException
    {
        public ParserException(string message, Location location) :
            base(ExceptionOrigin.Parser, message, location)
        {}
    }
}
