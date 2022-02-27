namespace Dutal.Interpreter.Parsing.Nodes
{
    class LogicalOr : BinaryNode
    {
        public LogicalOr(Node left, Node right)
            : base(left, right)
        {}

        public override string ToString() =>
            $"{base.ToString()} ||";
    }
}
