namespace Dutal.Interpreter.Evaluating.VariableTypes
{
    class NumberType : Variable
    {
        public float Value { get; set; }

        public NumberType(bool constant, float value) : base(constant) =>
            Value = value;

        public override string ToString() =>
            $"{Value} : Float {base.ToString()}";
    }
}
