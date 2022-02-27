namespace Dutal.Interpreter.Evaluating
{
    abstract class Variable
    {
        public bool Constant { get; }

        public Variable(bool constant) =>
            Constant = constant;

        public override string ToString() =>
            Constant ? "CONST" : "";
    }
}
