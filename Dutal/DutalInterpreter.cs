using System.Collections.Generic;

using Dutal.Interpreter.Input;
using Dutal.Interpreter.Tokenizing;
using Dutal.Interpreter.Parsing;
using Dutal.Interpreter.Evaluating;
using Dutal.Interpreter.Interfaces;

namespace Dutal
{
    class DutalInterpreter
    {
        private readonly Dictionary<string, Variable> variables;
        private readonly Dictionary<string, InterfaceFunction>? interfaces;
        private readonly string[]?  disabledFunctions;

        public DutalInterpreter(Dictionary<string, InterfaceFunction>? interfaces, string[]? functionBlacklist)
        {
            variables  = new Dictionary<string, Variable>();
            this.interfaces        = interfaces;
            this.disabledFunctions = functionBlacklist;
        }

        public void Interprete(string input)
        {
            InputReader inputReader = new InputReader(input);
            Tokenizer tokenizer     = new Tokenizer(inputReader, disabledFunctions);
            Parser parser           = new Parser(tokenizer, inputReader.location);
            Evaluator evaluator     = new Evaluator(variables, interfaces, inputReader.location);

            evaluator.Evaluate(parser.Parse());
        }
    }
}
