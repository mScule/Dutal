namespace Dutal.Interpreter.Parsing.Nodes
{
    class LogicalAnd : BinaryNode
    {
        public LogicalAnd(Node left, Node right)
            : base(left, right)
        {}

        public override string ToString() =>
            $"{base.ToString()} &&";
    }
}
