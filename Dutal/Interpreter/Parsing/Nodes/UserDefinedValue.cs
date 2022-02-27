namespace Dutal.Interpreter.Parsing.Nodes
{
    class UserDefinedValue : Node
    {
        public string UserDefinedValueName { get; }
        public UserDefinedValue(string userDefinedValueName) =>
            UserDefinedValueName = userDefinedValueName;

        public override string ToString() =>
            $"{base.ToString()} #{UserDefinedValueName}";
    }
}
