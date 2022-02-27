namespace Dutal.Interpreter.Parsing.Nodes
{
    class Relational : BinaryNode
    {
        public NodeOperator Operator { get; }
        
        public Relational(NodeOperator nodeOperator, Node left, Node right)
            : base(left, right) =>
            Operator = nodeOperator;

        public override string ToString() =>
            $"{base.ToString()} {Operator}";
    }
}
