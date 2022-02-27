using Dutal.Interpreter.Input;
using Dutal.Interpreter.Parsing;
using Dutal.Interpreter.Parsing.Nodes;
using Dutal.Interpreter.Evaluating.VariableTypes;
using Dutal.Interpreter.Exceptions;
using Dutal.Interpreter.Interfaces;

using System;
using System.Collections.Generic;

namespace Dutal.Interpreter.Evaluating
{
    class Evaluator
    {
        private readonly Location location;

        public Registers Registers { get; }
        public ParameterCache Parameters { get; }

        public Dictionary<string, Variable> Variables { get; }

        private Dictionary<string, InterfaceFunction>? Interfaces { get; }

        public Evaluator
        (
            Dictionary<string, Variable> variables,
            Dictionary <string, InterfaceFunction>? interfaces,
            Location location
        )
        {
            this.location = location;

            Registers  = new Registers(location);
            Parameters = new ParameterCache(location);

            this.Variables  = variables;
            this.Interfaces = interfaces;
        }

        public void Evaluate(Node ast)
        {
            if (ast is StatementList statementList)
                StatementList(statementList);

            else if (ast is ParameterList parameterList)
                ParameterList(parameterList);

            else if (ast is Parameter)
                Parameter();

            else if (ast is AssignmentStatement assignmentStatement)
                AssignmentStatement(assignmentStatement);

            else if (ast is UserDefinedFunctionDefinition userDefinedFunctionDefinition)
                UserDefinedFunctionDefinition(userDefinedFunctionDefinition);

            else if (ast is UserDefinedFunctionCall userDefinedFunction)
                UserDefinedFunction(userDefinedFunction);

            else if (ast is InterfaceCall interfaceCall)
                Interface(interfaceCall);

            else if (ast is FunctionCall function)
                Function(function);

            else if (ast is ReturnStatement returnStatement)
                ReturnStatement(returnStatement);

            else if (ast is LogicalOr logicalOr)
                LogicalOr(logicalOr);

            else if (ast is LogicalAnd logicalAnd)
                LogicalAnd(logicalAnd);

            else if (ast is Equality equality)
                Equality(equality);

            else if (ast is Concatenation concatenation)
                Concatenation(concatenation);

            else if (ast is Relational relational)
                Relational(relational);

            else if (ast is Expression expression)
                Expression(expression);

            else if (ast is Term term)
                Term(term);

            else if (ast is Unary unary)
                Unary(unary);

            else if (ast is NumberValue numberValue)
                NumberValue(numberValue);

            else if (ast is StringValue stringValue)
                StringValue(stringValue);

            else if (ast is UserDefinedValue userDefinedValue)
                UserDefinedValue(userDefinedValue);
        }

        // Node evaluating methods

        private void StatementList(StatementList statementList)
        {
            foreach (Node node in statementList.Children)
                Evaluate(node);
        }

        private void ParameterList(ParameterList parameterList)
        {
            Parameters.Clear();
            foreach(Node parameter in parameterList.Children)
                Parameters.Add(parameter);
        }

        private void Parameter() =>
            Registers.SetTree(new Parameter());

        private void AssignmentStatement(AssignmentStatement assignment)
        {
            void SetValue(string name, bool constant)
            {
                if (!Registers.IsNumberNull())
                    Variables[name] = new NumberType(constant, Registers.TryGetNumber());

                else if (!Registers.IsStringNull())
                    Variables[name] = new StringType(constant, Registers.TryGetString());

                else if (!Registers.IsTreeNull() && Registers.TryGetTree() is Parameter)
                {
                    Evaluate(Parameters.TryGet());
                    SetValue(name, constant);
                }
                else
                    throw new EvaluatorException("Constant can only be declared as number or string", location);
            }

            string name = assignment.UserDefinedName;

            if (Variables.ContainsKey(name) && Variables[name].Constant)
                throw new EvaluatorException("You can't redefine constant value", location);

            Evaluate(assignment.Value);

            switch(assignment.Operator)
            {
                case NodeOperator.ConstantAssignment:
                    SetValue(name, true);
                    break;

                case NodeOperator.VariableAssignment:
                    SetValue(name, false);
                    break;
            }
        }

        private void UserDefinedFunctionDefinition(UserDefinedFunctionDefinition userDefinedFunctionDefinition)
        {
            string name = userDefinedFunctionDefinition.FunctionName;

            if (Variables.ContainsKey(name))
                throw new EvaluatorException($"There is already definition with key {name}", location);

            Variables[name] = new TreeType(true, userDefinedFunctionDefinition.Statements);
        }

        private void UserDefinedFunction(UserDefinedFunctionCall userDefinedFunction)
        {
            Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

            string name = userDefinedFunction.Name;
            if (!Variables.ContainsKey(name) && Variables[name] is TreeType)
                throw new EvaluatorException($"{name} is not a user defined function", location);

            scope.Evaluate(userDefinedFunction.Parameters);

            TreeType body = (TreeType)Variables[name];
            scope.Evaluate(body.Value);

            VariableList.MergeToOuter(Variables, scope.Variables);

            Registers returnValue = scope.Registers;

            if (!returnValue.IsNumberNull())
                Registers.SetNumber(returnValue.TryGetNumber());
            else if (!returnValue.IsStringNull())
                Registers.SetString(returnValue.TryGetString());
            else if (!returnValue.IsTreeNull())
                Registers.SetTree(returnValue.TryGetTree());
        }

        private void Function(FunctionCall function)
        {
            Evaluate(function.Parameters);

            switch(function.FunctionName)
            {
                case "print":
                    string print = "";

                    while(Parameters.Count > 0)
                    {
                        Evaluate(Parameters.TryGet());

                        if (!Registers.IsNumberNull())
                            print += Registers.TryGetNumber();
                        else if (!Registers.IsStringNull())
                            print += Registers.TryGetString();
                        else if (!Registers.IsTreeNull())
                            throw new EvaluatorException("You can't print tree", location);
                    }

                    Console.Write(print);
                    break;

                case "input":
                    {
                        Node nameNode = Parameters.TryGet();

                        if (!(nameNode is UserDefinedValue))
                            throw new EvaluatorException("Given node has to be variable name", location);

                        UserDefinedValue variableNode = (UserDefinedValue)nameNode;

                        string name = variableNode.UserDefinedValueName;

                        if (Variables.ContainsKey(name) && Variables[name].Constant)
                            throw new EvaluatorException("You can't redefine constant value", location);

                        string stringValue = Console.ReadLine();

                        if (float.TryParse(stringValue, out float numberValue))
                            Variables[name] = new NumberType(true, numberValue);
                        else
                            Variables[name] = new StringType(true, stringValue);
                    }
                    break;

                case "times":
                    {
                        Evaluate(Parameters.TryGet());
                        float times = Registers.TryGetNumber();

                        if (times <= 0)
                            throw new EvaluatorException("Times can't be zero or less", location);

                        Node tree = Parameters.TryGet();

                        Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

                        for (int i = 0; i < (int)times; i++)
                            scope.Evaluate(tree);

                        VariableList.MergeToOuter(Variables, scope.Variables);
                    }
                    break;

                case "while":
                    {
                        Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

                        Node
                            condition = Parameters.TryGet(),
                            block     = Parameters.TryGet();

                        while(true)
                        {
                            scope.Evaluate(condition);
                            float boolean = scope.Registers.TryGetNumber();

                            if (!boolean.Equals(1))
                                break;

                            scope.Evaluate(block);
                        }

                        VariableList.MergeToOuter(Variables, scope.Variables);
                    }
                    break;

                case "if":
                    {
                        Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

                        Evaluate(Parameters.TryGet());
                        float boolean = Registers.TryGetNumber();

                        Node ifTrue = Parameters.TryGet();

                        if (boolean.Equals(1))
                            scope.Evaluate(ifTrue);

                        else if (Parameters.Count > 0)
                        {
                            Node ifFalse = Parameters.TryGet();
                            scope.Evaluate(ifFalse);
                        }

                        VariableList.MergeToOuter(Variables, scope.Variables);
                    }
                    break;

                case "chain":
                    {
                        Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

                        while (Parameters.Count > 0)
                        {
                            Evaluate(Parameters.TryGet());
                            float boolean = Registers.TryGetNumber();

                            Node ifTrue = Parameters.TryGet();

                            if (boolean.Equals(1))
                            {
                                scope.Evaluate(ifTrue);
                                break;
                            }
                        }

                        VariableList.MergeToOuter(Variables, scope.Variables);
                    }
                    break;

                case "step":
                    {
                        // Variablename (as variable)
                        Node nameNode = Parameters.TryGet();

                        if (!(nameNode is UserDefinedValue))
                            throw new EvaluatorException("Given node has to be variable name", location);

                        UserDefinedValue variableNode = (UserDefinedValue)nameNode;

                        string name = variableNode.UserDefinedValueName;

                        // Start value
                        Evaluate(Parameters.TryGet());
                        int start = (int)Registers.TryGetNumber();

                        // End value
                        Evaluate(Parameters.TryGet());
                        int end = (int)Registers.TryGetNumber();

                        // Step size
                        Evaluate(Parameters.TryGet());
                        int step = (int)Registers.TryGetNumber();

                        if (step <= 0)
                            throw new EvaluatorException("Step can't be negative or zero", location);

                        Node block = Parameters.TryGet();

                        Evaluator scope = new Evaluator(new Dictionary<string, Variable>(Variables), Interfaces, location);

                        // Rising
                        if (start < end)
                        {
                            for(int i = start; i < end; i += step)
                            {
                                scope.Variables[name] = new NumberType(true, i);
                                scope.Evaluate(block);
                            }
                        }

                        // Falling
                        else
                        {
                            for(int i = start; i > end; i -= step)
                            {
                                scope.Variables[name] = new NumberType(true, i);
                                scope.Evaluate(block);
                            }
                        }

                        VariableList.MergeToOuter(Variables, scope.Variables);
                    }
                    break;

                case "listvariables":
                    {
                        foreach(KeyValuePair<string, Variable> variable in Variables)
                        {
                            string key = variable.Key;

                            string value = variable.Value switch
                            {
                                NumberType numberType => numberType.ToString(),
                                StringType stringType => stringType.ToString(),
                                TreeType   treeType   => treeType.ToString(),
                                _                     => "Undefined"
                            };

                            Console.WriteLine($"{key} => {value}");
                        }
                    }
                    break;

                case "exit":
                    Console.ReadLine();
                    System.Environment.Exit(0);
                    break;

                default:
                    throw new EvaluatorException($"{function.FunctionName} is not a valid system function name", location);
            }    
        }

        private void Interface(InterfaceCall interfaceCall)
        {
            Evaluate(interfaceCall.Parameters);

            if (Interfaces == null)
                throw new EvaluatorException("There's no interfaces attached to the evaluator", location);

            if (!Interfaces.ContainsKey(interfaceCall.InterfaceName))
                throw new EvaluatorException($"There's no interface called {interfaceCall.InterfaceName}", location);

            Interfaces[interfaceCall.InterfaceName].Run(this);
        }

        private void ReturnStatement(ReturnStatement returnStatement) =>
            Evaluate(returnStatement.Value);

        private void LogicalOr(LogicalOr logicalOr)
        {
            Evaluate(logicalOr.Left);
            float left = Registers.TryGetNumber();

            Evaluate(logicalOr.Right);
            float right = Registers.TryGetNumber();

            if (left == 1 || right == 1)
                Registers.SetNumber(1);
            else
                Registers.SetNumber(0);
        }

        private void LogicalAnd(LogicalAnd logicalAnd)
        {
            Evaluate(logicalAnd.Left);
            float left = Registers.TryGetNumber();

            Evaluate(logicalAnd.Right);
            float right = Registers.TryGetNumber();

            if (left == 1 && right == 1)
                Registers.SetNumber(1);
            else
                Registers.SetNumber(0);
        }

        private void Equality(Equality equality)
        {
            string left = "", right = "";

            Evaluate(equality.Left);

            if (!Registers.IsStringNull())
                left = Registers.TryGetString();

            else if (!Registers.IsNumberNull())
                left = Registers.TryGetNumber() + "";

            else if (!Registers.IsTreeNull())
                throw new EvaluatorException("Tree cannot be checked for equality", location);

            Evaluate(equality.Right);

            if (!Registers.IsStringNull())
                right = Registers.TryGetString();

            else if (!Registers.IsNumberNull())
                right = Registers.TryGetNumber() + "";

            else if (!Registers.IsTreeNull())
                throw new EvaluatorException("Tree cannot be checked for equality", location);

            switch(equality.Operator)
            {
                case NodeOperator.Equal:
                    if (left == right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;

                case NodeOperator.NotEqual:
                    if (left != right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;
            }
        }
        
        private void Concatenation(Concatenation concatenation)
        {
            string left = "", right = "";

            Evaluate(concatenation.Left);

            if (!Registers.IsStringNull())
                left = Registers.TryGetString();

            else if (!Registers.IsNumberNull())
                left = Registers.TryGetNumber() + "";

            else if (!Registers.IsTreeNull())
                throw new EvaluatorException("Tree cannot be concatenated", location);

            Evaluate(concatenation.Right);

            if (!Registers.IsStringNull())
                right = Registers.TryGetString();

            else if (!Registers.IsNumberNull())
                right = Registers.TryGetNumber() + "";

            else if (!Registers.IsTreeNull())
                throw new EvaluatorException("Tree cannot be concatenated", location);

            Registers.SetString(left + right);
        }

        private void Relational(Relational relational)
        {
            Evaluate(relational.Left);
            float left = Registers.TryGetNumber();
            Evaluate(relational.Right);
            float right = Registers.TryGetNumber();

            switch(relational.Operator)
            {
                case NodeOperator.Smaller:
                    if (left < right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;

                case NodeOperator.Larger:
                    if (left > right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;

                case NodeOperator.SmallerOrEqual:
                    if (left <= right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;

                case NodeOperator.LargerOrEqual:
                    if (left >= right)
                        Registers.SetNumber(1);
                    else
                        Registers.SetNumber(0);
                    break;
            }
        }

        private void Expression(Expression expression)
        {
            Evaluate(expression.Left);
            float left = Registers.TryGetNumber();
            Evaluate(expression.Right);
            float right = Registers.TryGetNumber();

            switch(expression.Operator)
            {
                case NodeOperator.Addition:
                    Registers.SetNumber(left + right);
                    break;
                case NodeOperator.Subtraction:
                    Registers.SetNumber(left - right);
                    break;
            }
        }

        private void Term(Term term)
        {
            Evaluate(term.Left);
            float left = Registers.TryGetNumber();
            Evaluate(term.Right);
            float right = Registers.TryGetNumber();

            switch(term.Operator)
            {
                case NodeOperator.Multiplication:
                    Registers.SetNumber(left * right);
                    break;

                case NodeOperator.Division:
                    if (right.Equals(0))
                        throw new EvaluatorException("Trying to divide by zero", location);
                    Registers.SetNumber(left / right);
                    break;

                case NodeOperator.Modulus:
                    if (right.Equals(0))
                        throw new EvaluatorException("Trying to divide by zero", location);
                    Registers.SetNumber(left % right);
                    break;
            }
        }

        private void Unary(Unary unary)
        {
            Evaluate(unary.Child);

            switch(unary.Operator)
            {
                case NodeOperator.Addition:
                    Registers.SetNumber(+Registers.TryGetNumber());
                    break;

                case NodeOperator.Subtraction:
                    Registers.SetNumber(-Registers.TryGetNumber());
                    break;

                case NodeOperator.Not:
                    if (Registers.TryGetNumber().Equals(1))
                        Registers.SetNumber(0);

                    else if (Registers.TryGetNumber().Equals(0))
                        Registers.SetNumber(1);

                    else
                        throw new EvaluatorException("Not can be appiled only to values that are 1 or 0", location);
                    break;
            }
        }

        private void NumberValue(NumberValue numberValue) =>
            Registers.SetNumber(numberValue.Value);

        private void StringValue(StringValue stringValue) =>
            Registers.SetString(stringValue.Value);

        private void UserDefinedValue(UserDefinedValue userDefinedValue)
        {
            string key = userDefinedValue.UserDefinedValueName;
            if (Variables.ContainsKey(key))
            {
                Variable variable = Variables[key];

                if (variable is StringType stringType)
                    Registers.SetString(stringType.Value);

                else if (variable is NumberType numberType)
                    Registers.SetNumber(numberType.Value);

                else if (variable is TreeType treeType)
                    Registers.SetTree(treeType.Value);
            }
            else
                throw new EvaluatorException($"Variable with name {key}", location);
        }
    }
}
