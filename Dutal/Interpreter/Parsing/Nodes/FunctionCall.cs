namespace Dutal.Interpreter.Parsing.Nodes
{
    class FunctionCall : Node
    {
        public string FunctionName { get; }
        public Node Parameters { get; }

        public FunctionCall(string callName, Node parameters)
        {
            FunctionName = callName;
            Parameters = parameters;
        }

        public override string ToString() =>
            $"{base.ToString()} Function {FunctionName}";
    }
}
