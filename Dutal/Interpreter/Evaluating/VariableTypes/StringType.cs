namespace Dutal.Interpreter.Evaluating.VariableTypes
{
    class StringType : Variable
    {
        public string Value { get; set; }

        public StringType(bool constant, string value) : base(constant) =>
            Value = value;

        public override string ToString() =>
            $"{Value} : String {base.ToString()}";
    }
}
