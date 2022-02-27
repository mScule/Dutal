namespace Dutal.Interpreter.Parsing.Nodes
{
    class ParameterList : ListNode
    {
        public ParameterList(Node[] children)
            : base(children) {}

        public override string ToString() =>
            $"{base.ToString()} Parameter list";
    }
}
