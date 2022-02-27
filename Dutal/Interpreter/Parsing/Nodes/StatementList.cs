namespace Dutal.Interpreter.Parsing.Nodes
{
    class StatementList : ListNode
    {
        public StatementList(Node[] children)
            : base(children) {}

        public override string ToString() =>
            $"{base.ToString()} Statement list";
    }
}
