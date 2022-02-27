namespace Dutal.Interpreter.Parsing.Nodes
{
    class Equality : BinaryNode
    {
        public NodeOperator Operator { get; }

        public Equality(NodeOperator nodeOperator, Node left, Node right)
            : base(left, right) =>
            Operator = nodeOperator;

        public override string ToString() =>
            $"{base.ToString()} {Operator}";
    }
}
