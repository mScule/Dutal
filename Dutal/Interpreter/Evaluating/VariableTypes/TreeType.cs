using Dutal.Interpreter.Parsing;

namespace Dutal.Interpreter.Evaluating.VariableTypes
{
    class TreeType : Variable
    {
        public Node Value { get; }

        public TreeType(bool constant, Node value) : base(constant) =>
            Value = value;

        public override string ToString() =>
            $"{Value} : Tree {base.ToString()}";
    }
}
