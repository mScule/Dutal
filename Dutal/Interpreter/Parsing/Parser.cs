using System.Collections.Generic;

using Dutal.Interpreter.Input;
using Dutal.Interpreter.Tokenizing;
using Dutal.Interpreter.Parsing.Nodes;
using Dutal.Interpreter.Exceptions;

namespace Dutal.Interpreter.Parsing
{
    class Parser
    {
        // Tokens
        private static readonly Token
            openingParenthese   = new Token(TokenType.Symbol, "(" ),
            closingParenthese   = new Token(TokenType.Symbol, ")" ),
            openingCurlyBracket = new Token(TokenType.Symbol, "{" ),
            closingCurlyBracket = new Token(TokenType.Symbol, "}" ),
            separator           = new Token(TokenType.Symbol, ";" ),
            variableAssignment  = new Token(TokenType.Symbol, ":" ),
            constantAssignment  = new Token(TokenType.Symbol, "=" ),
            returnableValue     = new Token(TokenType.Symbol, "|" ),
            logicalAnd          = new Token(TokenType.Symbol, "&&"),
            logicalOr           = new Token(TokenType.Symbol, "||"),
            not                 = new Token(TokenType.Symbol, "!" ),
            equal               = new Token(TokenType.Symbol, "=="),
            notEqual            = new Token(TokenType.Symbol, "!="),
            concatenation       = new Token(TokenType.Symbol, "&" ),
            smaller             = new Token(TokenType.Symbol, "<" ),
            larger              = new Token(TokenType.Symbol, ">" ),
            smallerOrEqual      = new Token(TokenType.Symbol, "<="),
            largerOrEqual       = new Token(TokenType.Symbol, ">="),
            addition            = new Token(TokenType.Symbol, "+" ),
            subtraction         = new Token(TokenType.Symbol, "-" ),
            multiplication      = new Token(TokenType.Symbol, "*" ),
            division            = new Token(TokenType.Symbol, "/" ),
            modulus             = new Token(TokenType.Symbol, "%" );

        private readonly Location location;
        private readonly Tokenizer tokenizer;

        public Parser(Tokenizer tokenizer, Location location)
        {
            this.tokenizer = tokenizer;
            this.location = location;
        }

        public Node Parse() => StatementListGlobal();

        // Grammar methods

        // StatementListGlobal : Statement ((';') Statement)*
        private Node StatementListGlobal()
        {
            List<Node> statements = new List<Node>
            {
                Statement()
            };

            while (tokenizer.Current.Equals(separator))
            {
                tokenizer.Next();
                statements.Add(Statement());
            }

            return new StatementList(statements.ToArray());
        }

        // StatementList : '{' Statement ((';') Statement)* '}'
        private Node StatementList()
        {
            DemandByToken(openingCurlyBracket);
            tokenizer.Next();

            List<Node> statements = new List<Node>
            {
                Statement()
            };

            while (tokenizer.Current.Equals(separator))
            {
                tokenizer.Next();
                statements.Add(Statement());
            }

            DemandByToken(closingCurlyBracket);
            tokenizer.Next();

            return new StatementList(statements.ToArray());
        }

        // ParameterList : '(' Expression ((';') Expression)* ')'
        private Node ParameterList()
        {
            DemandByToken(openingParenthese);
            tokenizer.Next();

            List<Node> parameters = new List<Node>();

            if (!tokenizer.Current.Equals(closingParenthese))
            {
                parameters.Add(Value());

                while (tokenizer.Current.Equals(separator))
                {
                    tokenizer.Next();
                    parameters.Add(Value());
                }
            }
            DemandByToken(closingParenthese);
            tokenizer.Next();

            return new ParameterList(parameters.ToArray());
        }

        // Statement : #UserDefined : Value       // Variable definition
        //           | #UserDefined = Value       // Const    definition
        //           | #UserDefined StatementList // Function definition
        //           | Call
        private Node Statement()
        {
            if (tokenizer.Current.Type.Equals(TokenType.UserDefined))
            {
                string name = tokenizer.Current.Value;
                tokenizer.Next();

                // Variable definition
                if (tokenizer.Current.Equals(variableAssignment))
                {
                    tokenizer.Next();
                    return new AssignmentStatement(NodeOperator.VariableAssignment, name, Value());
                }

                // Const definition
                else if (tokenizer.Current.Equals(constantAssignment))
                {
                    tokenizer.Next();
                    return new AssignmentStatement(NodeOperator.ConstantAssignment, name, Value());
                }

                // Function definition
                else if (tokenizer.Current.Equals(openingCurlyBracket))
                {
                    return new UserDefinedFunctionDefinition(name, StatementList());
                }

                // User defined function call
                else
                    return new UserDefinedFunctionCall(name, ParameterList());
            }

            // Function call
            else if (tokenizer.Current.Type.Equals(TokenType.Word))
            {
                string name = tokenizer.Current.Value;
                tokenizer.Next();
                return new FunctionCall(name, ParameterList());
            }

            // Interface function call
            else if (tokenizer.Current.Type.Equals(TokenType.Interface))
                return InterfaceCall();

            // Return statement
            else if (tokenizer.Current.Equals(returnableValue))
            {
                tokenizer.Next();
                return new ReturnStatement(Value());
            }
            else
                throw new ParserException("Statement has to start with UserDefined or Call", location);
        }

        // Value : LogicalOr | StatementList
        private Node Value()
        {
            if (tokenizer.Current.Equals(openingCurlyBracket))
                return StatementList();
            else
                return LogicalOr();
        }

        // LogicalOr : LogicalAnd (('||') LogicalOr) *
        private Node LogicalOr()
        {
            Node node = LogicalAnd();

            while(tokenizer.Current.Equals(logicalOr))
            {
                tokenizer.Next();
                node = new LogicalOr(node, LogicalAnd());
            }

            return node;
        }

        // LogicalAnd : Equality (('&&') Equality) *
        private Node LogicalAnd()
        {
            Node node = Not();

            while(tokenizer.Current.Equals(logicalAnd))
            {
                tokenizer.Next();
                node = new LogicalAnd(node, Not());
            }

            return node;
        }

        // Not : ('!')* Expression
        private Node Not()
        {
            if(tokenizer.Current.Equals(not))
            {
                tokenizer.Next();
                return new Unary(NodeOperator.Not, Equality());
            }
            else
                return Equality();
        }

        // Equality : Relational (('=='|'!=') Relational) *
        private Node Equality()
        {
            Node node = Concatenation();

            while
            (
                tokenizer.Current.Equals(equal) ||
                tokenizer.Current.Equals(notEqual)
            )
            {
                NodeOperator nodeOperator = tokenizer.Current.Value switch
                {
                    "==" => NodeOperator.Equal,
                    "!=" => NodeOperator.NotEqual,
                    _    => NodeOperator.Undefined
                };

                tokenizer.Next();

                node = new Equality(nodeOperator, node, Concatenation());
            }

            return node;
        }

        // Concatenation : Relational (('&') Relational) *
        private Node Concatenation()
        {
            Node node = Relational();

            while (tokenizer.Current.Equals(concatenation))
            {
                tokenizer.Next();
                node = new Concatenation(node, Relational());
            }

            return node;
        }

        // Relational : Expression (('<'|'>'|'<='|'>=') Expression) * 
        private Node Relational()
        {
            Node node = Expression();

            while
            (
                tokenizer.Current.Equals(smaller)        ||
                tokenizer.Current.Equals(larger)         ||
                tokenizer.Current.Equals(smallerOrEqual) ||
                tokenizer.Current.Equals(largerOrEqual)
            )
            {
                NodeOperator nodeOperator = tokenizer.Current.Value switch
                {
                    "<"  => NodeOperator.Smaller,
                    ">"  => NodeOperator.Larger,
                    "<=" => NodeOperator.SmallerOrEqual,
                    ">=" => NodeOperator.LargerOrEqual,
                    _    => NodeOperator.Undefined
                };

                tokenizer.Next();

                node = new Relational(nodeOperator, node, Expression());
            }

            return node;
        }

        // Expression : Term (('+'|'-') Term) *
        private Node Expression()
        {
            Node node = Term();

            while
            (
                tokenizer.Current.Equals(addition) ||
                tokenizer.Current.Equals(subtraction)
            )
            {
                NodeOperator nodeOperator = tokenizer.Current.Value switch
                {
                    "+" => NodeOperator.Addition,
                    "-" => NodeOperator.Subtraction,
                    _   => NodeOperator.Undefined
                };

                tokenizer.Next();

                node = new Expression(nodeOperator, node, Term());
            }

            return node;
        }

        // Term : Factor (('*'|'/'|'%') Factor) *
        private Node Term()
        {
            Node node = Factor();

            while
            (
                tokenizer.Current.Equals(multiplication) ||
                tokenizer.Current.Equals(division)       ||
                tokenizer.Current.Equals(modulus)
            )
            {
                NodeOperator nodeOperator = tokenizer.Current.Value switch
                {
                    "*" => NodeOperator.Multiplication,
                    "/" => NodeOperator.Division,
                    "%" => NodeOperator.Modulus,
                    _   => NodeOperator.Undefined
                };

                tokenizer.Next();
                node = new Term(nodeOperator, node, Factor());
            }

            return node;
        }

        // Factor : '(' Value ')'
        //        | '+'
        //        | '-'
        //        | Num
        //        | UserDefined
        //        | @InterfaceCall()
        private Node Factor()
        {
            switch (tokenizer.Current.Type)
            {
                case TokenType.Symbol:

                    switch (tokenizer.Current.Value)
                    {
                        // Unary addition
                        case "+":
                            tokenizer.Next();
                            return new Unary(NodeOperator.Addition, Factor());

                        // Unary subtraction
                        case "-":
                            tokenizer.Next();
                            return new Unary(NodeOperator.Subtraction, Factor());

                        // Expression
                        case "(":
                            tokenizer.Next();
                            Node value = Value();
                            DemandByToken(closingParenthese);
                            tokenizer.Next();
                            return value;

                        case "param":
                            tokenizer.Next();
                            return new Parameter();

                        default:
                            throw new ParserException(
                                $"Waited for +, -, or ( Symbol but got {tokenizer.Current.Value}",
                                location
                            );
                    }

                // Number
                case TokenType.Number:
                    float numericValue = float.Parse(tokenizer.Current.Value);
                    tokenizer.Next();
                    return new NumberValue(numericValue);

                // String
                case TokenType.String:
                    string stringValue = tokenizer.Current.Value;
                    tokenizer.Next();
                    return new StringValue(stringValue);

                // Userdefined function call, or variable
                case TokenType.UserDefined:
                    return UserDefined();

                // Interface function call
                case TokenType.Interface:
                    return InterfaceCall();

                default:
                    throw new ParserException(
                        $"Waited for Symbol, Number, or UserDefined but got {tokenizer.Current.Type}",
                        location
                    );
            }
        }

        // UserDefined : #UserDefinedCall()
        //             | #UserDefinedValue 
        private Node UserDefined()
        {
            string name = tokenizer.Current.Value;
            tokenizer.Next();

            // UserDefinedCall
            if (tokenizer.Current.Equals(openingParenthese))
                return new UserDefinedFunctionCall(name, ParameterList());

            // UserDefinedValue
            else
                return new UserDefinedValue(name);
        }

        private Node InterfaceCall()
        {
            string name = tokenizer.Current.Value;
            tokenizer.Next();

            DemandByToken(openingParenthese);
            return new InterfaceCall(name, ParameterList());
        }

        // Token evaluation methods
        private void DemandByToken(Token token)
        {
            if (!tokenizer.Current.Equals(token))
            {
                throw new ParserException(
                    $"Demanded token was {token}, but {tokenizer.Current} was given",
                    location
                );
            }
        }
    }
}
