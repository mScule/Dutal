namespace Dutal.Interpreter.Parsing.Nodes
{
    class Term : BinaryNode
    {
        public NodeOperator Operator { get; }

        public Term(NodeOperator nodeOperator, Node left, Node right)
            : base(left, right) =>
            Operator = nodeOperator;

        public override string ToString() =>
            $"{base.ToString()} {Operator}";
    }
}
