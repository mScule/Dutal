namespace Dutal.Interpreter.Parsing.Nodes
{
    class NumberValue : LeafNode
    {
        public float Value { get; }

        public NumberValue (float value) =>
            Value = value;

        public override string ToString() =>
            $"{base.ToString()} {Value}";
    }
}
