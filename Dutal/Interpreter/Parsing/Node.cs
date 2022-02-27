namespace Dutal.Interpreter.Parsing
{
    enum NodeOperator
    {
        VariableAssignment, // :
        ConstantAssignment, // =
        Addition,           // +
        Subtraction,        // -
        Multiplication,     // *
        Division,           // /
        Modulus,            // %
        Concatenation,      // &
        Not,                // !
        Smaller,            // <
        Larger,             // >
        SmallerOrEqual,     // <=
        LargerOrEqual,      // >=
        Equal,              // ==
        NotEqual,           // !=
        LogicalAnd,         // &&
        LogicalOr,          // ||
        Undefined
    }

    abstract class Node
    {
        public override string ToString() => "Node";
    }

    class LeafNode : Node
    {
        public override string ToString() =>
            $"{base.ToString()} Leaf";
    }

    class UnaryNode : Node
    {
        public Node Child { get; }

        public UnaryNode(Node child) =>
            Child = child;

        public override string ToString() =>
            $"{base.ToString()} Unary";
    }

    class BinaryNode : Node
    {
        public Node Left { get; }
        public Node Right { get; }

        public BinaryNode(Node left, Node right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString() =>
            $"{base.ToString()} Binary";
    }

    class ListNode : Node
    {
        public Node[] Children { get; }

        public ListNode(Node[] children) =>
            Children = children;

        public override string ToString() =>
            $"{base.ToString()} List";
    }
}
