namespace Dutal.Interpreter.Parsing.Nodes
{
    class InterfaceCall : Node
    {
        public string InterfaceName { get; }
        public Node Parameters { get; }

        public InterfaceCall(string interfaceName, Node parameters)
        {
            InterfaceName = interfaceName;
            Parameters = parameters;
        }

        public override string ToString() =>
            $"{base.ToString()} Interface {InterfaceName}";
    }
}
