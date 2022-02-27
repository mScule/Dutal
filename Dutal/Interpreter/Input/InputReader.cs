using Dutal.Interpreter.Exceptions;

namespace Dutal.Interpreter.Input
{
    class InputReader
    {
        public readonly Location location = new Location();
        public char Current { get => current; }

        private readonly string input = "";
        private int i;
        private char current;

        public InputReader(string input)
        {
            if (input != null && input.Length > 0)
            {
                this.input += input;
                i = 0;
                current = input[i];
            }
            else
                throw new InputReaderException("Input can't be empty or null", location);
        }

        public void Next()
        {
            i++;

            if (i < input.Length)
                current = input[i];
            else
                current = '\0';

            if (current.Equals('\n'))
                location.NextLine();
            else
                location.NextChar();
        }

        public char Peek(int offset)
        {
            int peek = i + offset;

            if (peek < input.Length)
                return input[peek];
            else
                return '\0';
        }
    }
}
