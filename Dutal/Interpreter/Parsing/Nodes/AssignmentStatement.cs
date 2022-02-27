namespace Dutal.Interpreter.Parsing.Nodes
{
    class AssignmentStatement : Node
    {
        public string UserDefinedName { get; }
        public Node Value { get; }
        public NodeOperator Operator { get; }

        public AssignmentStatement(NodeOperator nodeOperator, string userDefinedName, Node value)
        {
            UserDefinedName = userDefinedName;
            Value = value;
            Operator = nodeOperator;
        }

        public override string ToString() =>
            $"{base.ToString()} {Operator} {UserDefinedName}";
    }
}
