using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Token
    {
        public string value { get; set; }
        public enum TokenType { VAR,OPERATIORS,WHILE,IF,PRINT,LEFT_PAR,RIGHT_PAR,LEFT_BRACKET,RIGHT_BRACKET,TRUTH_ASSIGNMENT,ASSIGNMENT_OPERATOR,INTERGER,TYPENAME,SIMI_COLON}
        public TokenType Type { get; set; }
    }

    class TokenQueu
    {
        private List<Token> tokens;
        private int position;
        public bool AtEnd { get { return tokens.Count > position; } }
        public TokenQueu()
        {
            tokens = new List<Token>();
            position = 0;
        }

        public void EnQueu(Token newToken)
        {
            tokens.Add(newToken);
        }

        public Token Next() {
            position++;
             return tokens[position - 1];
        }

        public Token Peek()
        {
            return tokens[position];
        }

        public void Dump()
        {
            foreach(var tok in tokens)
            {
                Console.WriteLine(tok.Type);
            }
        }
    }
}
