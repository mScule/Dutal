namespace Dutal.Interpreter.Parsing.Nodes
{
    class Unary : UnaryNode
    {
        public NodeOperator Operator { get; }

        public Unary(NodeOperator nodeOperator, Node child)
            : base(child) =>
            Operator = nodeOperator;

        public override string ToString() =>
            $"{base.ToString()} {Operator}";
    }
}
