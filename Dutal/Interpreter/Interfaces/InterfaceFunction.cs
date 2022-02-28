using Dutal.Interpreter.Evaluating;

namespace Dutal.Interpreter.Interfaces
{
    class InterfaceFunction : IInterfaceFunction
    {
        private Evaluator? evaluator;

        protected string TryGetStringParameter()
        {
            evaluator?.Evaluate(evaluator.Parameters.TryGet());
            string? value = evaluator?.Registers.TryGetString();

            return value ?? "";
        }

        protected float TryGetNumberParameter()
        {
            evaluator?.Evaluate(evaluator.Parameters.TryGet());
            float? value = evaluator?.Registers.TryGetNumber();

            return value ?? 0.0f;
        }

        protected void SetStringReturnValue(string value) =>
            evaluator?.Registers.SetString(value);

        protected void SetNumberReturnValue(float value) =>
            evaluator?.Registers.SetNumber(value);

        public virtual void Run(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }
    }
}
