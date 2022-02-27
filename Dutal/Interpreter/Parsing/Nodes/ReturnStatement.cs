namespace Dutal.Interpreter.Parsing.Nodes
{
    class ReturnStatement : Node
    {
        public Node Value { get; }

        public ReturnStatement(Node value) =>
            Value = value;

        public override string ToString() =>
            $"{base.ToString()} Return statement";
    }
}
