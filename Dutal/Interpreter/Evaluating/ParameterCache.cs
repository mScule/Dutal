using System.Collections.Generic;

using Dutal.Interpreter.Input;
using Dutal.Interpreter.Parsing;
using Dutal.Interpreter.Exceptions;

namespace Dutal.Interpreter.Evaluating
{
    class ParameterCache
    {
        private readonly Location location;
        private readonly Queue<Node> parameterCache;

        public ParameterCache(Location location)
        {
            this.location = location;
            parameterCache = new Queue<Node>();
        }

        public Node TryGet()
        {
            if (parameterCache.Count > 0)
                return parameterCache.Dequeue();
            else
                throw new EvaluatorException("There wasn't no given parameter to be called", location);
        }

        public Node TryGet(string errorMessage)
        {
            if (parameterCache.Count > 0)
                return parameterCache.Dequeue();
            else
                throw new EvaluatorException(errorMessage, location);
        }

        public int Count { get => parameterCache.Count; }

        public void Add(Node parameter) =>
            parameterCache.Enqueue(parameter);

        public void Clear() =>
            parameterCache.Clear();
    }
}
