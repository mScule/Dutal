namespace Dutal.Interpreter.Input
{
    class Location
    {
        public int Line { get => ln; }
        public int Char { get => ch; }

        private int ln, ch;

        public Location() =>
            ln = ch = 1;

        public void NextLine()
        {
            ln++;
            ch = 1;
        }

        public void NextChar() =>
            ch++;

        public override string ToString() =>
            $"(Line {Line} Char {Char})";
    }
}
