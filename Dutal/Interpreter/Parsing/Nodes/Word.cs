namespace Dutal.Interpreter.Parsing.Nodes
{
    class Word : LeafNode
    {
        public string Value { get; }

        public Word(string value) =>
            Value = value;

        public override string ToString() =>
            $"{base.ToString()} {Value}";
    }
}
