using System;
using System.IO;
using System.Collections.Generic;

using Dutal;
using Dutal.Interpreter.Evaluating;
using Dutal.Interpreter.Interfaces;

class ExampleInterface : InterfaceFunction
{
    public override void Run(Evaluator evaluator)
    {
        base.Run(evaluator);
        float number = TryGetNumberParameter();
        SetStringReturnValue($"You gave: {number}");
    }
}

class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, InterfaceFunction> interfaces = new Dictionary<string, InterfaceFunction>
        {
            ["test"] = new ExampleInterface()
        };

        string[] disabledFunctions = { "listvariables" };
        DutalInterpreter dutal = new DutalInterpreter(interfaces, disabledFunctions);

        string input = "";

        if(args.Length > 0 && File.Exists(args[0]))
            input = File.ReadAllText(args[0]);

        Console.WriteLine("Dutal example implementation");

        while(true)
        {
            try
            {
                if (input.Equals(""))
                {
                    Console.WriteLine("");
                    input = Console.ReadLine();
                }

                dutal.Interprete(input);
                input = "";
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error {e.Message}");
                input = "";
            }
        }
    }
}
