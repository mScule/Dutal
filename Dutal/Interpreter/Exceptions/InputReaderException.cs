using Dutal.Interpreter.Input;

namespace Dutal.Interpreter.Exceptions
{
    class InputReaderException : InterpreterException
    {
        public InputReaderException(string message, Location location) :
            base(ExceptionOrigin.Input, message, location)
        {}
    }
}
