using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Node
    {
        public string TypeName { get; set; }
        public string Value { get; set; } = "";
        public enum NodeType { OPERATORS, INT, EXPRESSION, BOOLEAN_OP, TYPE, ASSIGNMENT, TRUTH_VALUE, TRUTH_ASSIGNMENT, PRINT_STATEMENT, CODE_BLOCK, LOOP, IF,VAR,TYPENAME,INTERGER ,DECLARATION, ASSIGNMENT_OPERATOR,WHILE};
        public NodeType nodeType { get; set; }
        public Node[] Children = new Node[0];

        void indent(int offset)
        {
            for(int i = 0;i < offset * 4; i++)
            {
                Console.Write(" ");
            }
        }

        public void dump(int offset = 0)
        {
            indent(offset);
            Console.Write(TypeName);
            if (!string.IsNullOrEmpty(Value))
            {
                Console.WriteLine(" "+Value);
            }
            else
            {
                Console.WriteLine();
            }
            foreach(var child in Children)
            {
                child.dump(offset + 1);
            }
        }
    }
}
