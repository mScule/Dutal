using Dutal.Interpreter.Input;

namespace Dutal.Interpreter.Exceptions
{
    class EvaluatorException : InterpreterException
    {
        public EvaluatorException(string message, Location location) :
            base(ExceptionOrigin.Evaluator, message, location)
        {}
    }
}
