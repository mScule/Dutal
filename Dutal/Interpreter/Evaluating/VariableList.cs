using System.Collections.Generic;

namespace Dutal.Interpreter.Evaluating
{
    static class VariableList
    {
        public static void MergeToOuter(Dictionary<string, Variable> outer, Dictionary<string, Variable> inner)
        {
            foreach(KeyValuePair<string, Variable> variable in inner)
                if (outer.ContainsKey(variable.Key))
                    outer[variable.Key] = variable.Value;
        }
    }
}
