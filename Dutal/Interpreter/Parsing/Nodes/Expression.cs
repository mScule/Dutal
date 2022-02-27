namespace Dutal.Interpreter.Parsing.Nodes
{
    class Expression : BinaryNode
    {
        public NodeOperator Operator { get; }

        public Expression(NodeOperator nodeOperator, Node left, Node right)
            : base(left, right) =>
            Operator = nodeOperator;

        public override string ToString() =>
            $"{base.ToString()} {Operator}";
    }
}
