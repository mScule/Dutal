using Dutal.Interpreter.Input;
using Dutal.Interpreter.Exceptions;

namespace Dutal.Interpreter.Tokenizing
{
    class Tokenizer
    {
        private readonly InputReader input;
        private readonly string[]?   disabledFunctions;

        public Tokenizer(InputReader input, string[]? functionBlacklist)
        {
            this.input = input;
            this.disabledFunctions = functionBlacklist;
            Next();
        }

        public  Token Current { get => current; }
        private Token current;

        public void Next() => NextToken();

        private void NextToken()
        {
        Start:;
            // Blanks
            SkipBlanks();

            // Comments
            if (input.Current.Equals('?'))
            {
                SkipComment();
                goto Start;
            }

            // Words
            if (IsAlphabet(input.Current))
            {
                string word = BuildWord();

                if (disabledFunctions != null)
                    foreach (string blacklisted in disabledFunctions)
                        if (word.Equals(blacklisted))
                            throw new TokenizerException($"Function {word} is disabled", input.location);

                current = word switch
                {
                    "param" => new Token(TokenType.Symbol, word),
                    _ => new Token(TokenType.Word, word)
                };
            }

            // Numbers
            else if (IsNumber(input.Current))
                current = new Token(TokenType.Number, BuildNumber());

            // Strings
            else if (input.Current.Equals('"'))
                current = new Token(TokenType.String, BuildString());

            // Variables
            else if (input.Current.Equals('#'))
                current = new Token(TokenType.UserDefined, BuildVariable());

            // Interface
            else if (input.Current.Equals('@'))
                current = new Token(TokenType.Interface, BuildVariable());

            // End of file
            else if (input.Current.Equals('\0'))
                current = new Token(TokenType.EndOfFile, "EndOfFile");

            // Symbols
            else
                current = new Token(TokenType.Symbol, BuildSymbol());
        }

        // Skip methods
        private void SkipBlanks()
        {
            while (IsBlank(input.Current))
                input.Next();
        }

        private void SkipComment()
        {
            while (!input.Current.Equals('\n') && !input.Current.Equals('\0'))
                input.Next();
        }

        // Build methods
        private string BuildWord()
        {
            string word = "" + input.Current;
            input.Next();

            while (IsAlphanumeric(input.Current) || input.Current.Equals('_'))
            {
                word += input.Current;
                input.Next();
            }

            return word;
        }

        private string BuildVariable()
        {
            input.Next();
            if (IsAlphanumeric(input.Current) || input.Current.Equals('_'))
                return BuildWord();
            else
                throw new TokenizerException(
                    "Variable name is missing",
                    input.location
                );
        }

        private string BuildNumber()
        {
            string number = "" + input.Current;
            input.Next();

            while (IsNumber(input.Current))
            {
                number += input.Current;
                input.Next();
            }

            if (input.Current.Equals('.'))
            {
                number += input.Current;
                input.Next();

                if (!IsNumber(input.Current))
                {
                    throw new TokenizerException(
                        "There wasn't number after the decimal point",
                        input.location
                    );
                }

                while (IsNumber(input.Current))
                {
                    number += input.Current;
                    input.Next();
                }
            }

            return number;
        }

        private string BuildString()
        {
            string str = "";
            input.Next();

            while (!input.Current.Equals('"'))
            {
                // Escape characters
                if (input.Current.Equals('\\'))
                {
                    input.Next();

                    str += input.Current switch
                    {
                        '"'  => '"',
                        'n'  => '\n',
                        't'  => '\t',
                        '\\' => '\\',
                        _    => throw new TokenizerException(
                            $"Unsupported escape character {input.Current}",
                            input.location
                        )
                    };
                }
                else if (input.Current.Equals('\0'))
                {
                    throw new TokenizerException(
                       "String wasn't closed",
                       input.location
                    );
                }
                else
                    str += input.Current;

                input.Next();
            }

            input.Next();

            return str;
        }

        private string BuildSymbol()
        {
            string symbol = "";

            switch (input.Current)
            {
                case '*': // Multiplication
                case '/': // Division
                case '%': // Modulus
                case ':': // Variable assignment
                case ';': // End statement, Parameter separator
                case '(': // Opening parenthese
                case ')': // Closing parenthese
                case '[': // Opening bracket
                case ']': // Closing bracket
                case '{': // Opening block
                case '}': // Closing block
                    symbol += input.Current;
                    break;

                case '+': // Increment   ++ | Addition            +
                case '-': // Decrement   -- | Subtraction         -
                case '&': // Logical and && | Concatenation       &
                case '|': // Logical or  || | Return              |
                case '=': // Equals      == | Constant Assignment =

                    // Two characters
                    if (input.Peek(1).Equals(input.Current))
                    {
                        symbol = input.Current + "" + input.Current;
                        input.Next();
                    }

                    // Single character
                    else
                        symbol += input.Current;
                    break;

                case '<': // Smaller or equal <= | Smaller <
                case '>': // Larger or equal  >= | Larger  >
                case '!': // Not equal        != | Not     !

                    // Two characters
                    if (input.Peek(1).Equals('='))
                    {
                        symbol = input.Current + "=";
                        input.Next();
                    }

                    // Single character
                    else
                        symbol += input.Current;
                    break;

                default:
                    throw new TokenizerException(
                        $"Symbol \"{input.Current}\" is not supported",
                        input.location
                    );
            }

            input.Next();
            return symbol;
        }

        // Char evaluation methods
        private bool IsBlank(char c) =>
            c.Equals(' ')  ||
            c.Equals('\n') ||
            c.Equals('\r') ||
            c.Equals('\t');

        private bool IsNumber(char c) =>
            c >= '0' && c <= '9';

        private bool IsAlphabet(char c) =>
            c >= 'a' && c <= 'z' ||
            c >= 'A' && c <= 'Z';

        private bool IsAlphanumeric(char c) =>
            IsAlphabet(c) || IsNumber(c);
    }
}
