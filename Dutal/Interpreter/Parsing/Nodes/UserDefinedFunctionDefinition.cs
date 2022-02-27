namespace Dutal.Interpreter.Parsing.Nodes
{
    class UserDefinedFunctionDefinition : Node
    {
        public string FunctionName { get; }

        public Node Statements { get; }

        public UserDefinedFunctionDefinition(string functionName, Node statements)
        {
            FunctionName = functionName;
            Statements   = statements;
        }

        public override string ToString() =>
            $"{base.ToString()} System function {FunctionName}";
    }
}
