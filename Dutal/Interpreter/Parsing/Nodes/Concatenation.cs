namespace Dutal.Interpreter.Parsing.Nodes
{
    class Concatenation : BinaryNode
    {
        public Concatenation(Node left, Node right)
            : base(left, right)
        {}

        public override string ToString() =>
            $"{base.ToString()} &";
    }
}
