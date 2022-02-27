using System;
using Dutal.Interpreter.Input;

namespace Dutal.Interpreter.Exceptions
{
    enum ExceptionOrigin
    {
        Input,
        Tokenizer,
        Parser,
        Evaluator
    }

    class InterpreterException : Exception
    {
        public int Line { get; }
        public int Char { get; }

        public InterpreterException(ExceptionOrigin origin, String message, Location location)
            : base($"{origin}: {message} {location}")
        {}
    }
}
