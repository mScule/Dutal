using Dutal.Interpreter.Input;
using Dutal.Interpreter.Parsing;
using Dutal.Interpreter.Exceptions;

namespace Dutal.Interpreter.Evaluating
{
    class Registers
    {
        private Location location;

        private string? stringRegister;
        private float?  numberRegister;
        private Node?   treeRegister;

        public Registers(Location location)
        {
            this.location = location;

            stringRegister = null;
            numberRegister = null;
            treeRegister   = null;
        }

        public bool IsStringNull() =>
            stringRegister == null;

        public bool IsNumberNull() =>
            numberRegister == null;

        public bool IsTreeNull() =>
            treeRegister == null;

        public string TryGetString()
        {
            if (stringRegister == null)
                throw new EvaluatorException("Wanted string", location);

            return stringRegister;
        }

        public float TryGetNumber()
        {
            if (numberRegister == null)
                throw new EvaluatorException("Wanted number", location);

            return (float)numberRegister;
        }

        public Node TryGetTree()
        {
            if (treeRegister == null)
                throw new EvaluatorException("Wanted tree", location);

            return treeRegister;
        }

        public void SetString(string? value)
        {
            InitRegisters();
            stringRegister = value;
        }

        public void SetNumber(float? value)
        {
            InitRegisters();
            numberRegister = value;
        }

        public void SetTree(Node? value)
        {
            InitRegisters();
            treeRegister = value;
        }

        private void InitRegisters()
        {
            stringRegister = null;
            numberRegister = null;
            treeRegister   = null;
        }
    }
}
