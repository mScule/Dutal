namespace Dutal.Interpreter.Parsing.Nodes
{
    class StringValue : LeafNode
    {
        public string Value { get; }

        public StringValue(string value) =>
            Value = value;

        public override string ToString() =>
            $"{base.ToString()} {Value}";
    }
}
