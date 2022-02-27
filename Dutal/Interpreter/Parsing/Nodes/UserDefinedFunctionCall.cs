namespace Dutal.Interpreter.Parsing.Nodes
{
    class UserDefinedFunctionCall : Node
    {
        public string Name { get; }
        public Node Parameters { get; }

        public UserDefinedFunctionCall(string name, Node parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public override string ToString() =>
            $"{base.ToString()} User defined function {Name}";
    }
}
