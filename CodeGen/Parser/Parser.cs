using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class DescentParser
    {
        /// <summary>
        /// Queu that holds all the tokens
        /// </summary>
        TokenQueu Queue;
        public DescentParser(TokenQueu Queue)
        {
            this.Queue = Queue;
        }

        /// <summary>
        /// Gust reads a operator into the tree
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node OPERATORS(Token next)
        {
            if (next.Type == Token.TokenType.OPERATIORS)
            {
                return new Node
                {
                    TypeName = "Operator",
                    Value = next.value,
                    nodeType = Node.NodeType.OPERATORS
                };
            }
            else
            {
                throw new Exception("ParseError");
            }
        }

        /// <summary>
        /// Reads a Int into the tree
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node INTERGER(Token next)
        {
            if (next.Type == Token.TokenType.INTERGER)
            {
                return new Node
                {
                    TypeName = "INT",
                    Value = next.value,
                    nodeType = Node.NodeType.INTERGER
                };
            }
            else
            {
                throw new Exception("ParseError");
            }
        }

        public Node STRING(Token next)
        {
            return new Node
            {
                TypeName = "STRING",
                Value = next.value,
                nodeType = Node.NodeType.STRING
            };
        }
        

        /// <summary>
        /// Reads an assignment into the tree
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node ASSIGNMENT_OPERATORS(Token next)
        {
            if (next.Type == Token.TokenType.ASSIGNMENT_OPERATOR)
            {
                return new Node
                {
                    TypeName = "ASSIGNMENT_OPERATOR",
                    Value = next.value,
                    nodeType = Node.NodeType.ASSIGNMENT_OPERATOR
                };
            }
            else
            {
                throw new Exception("ParseError");
            }
        }

        /// <summary>
        /// Reads a Var into the tree
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node VAR(Token next)
        {
            if (next.Type == Token.TokenType.VAR)
            {
                return new Node
                {
                    TypeName = "VAR",
                    Value = next.value,
                    nodeType = Node.NodeType.VAR
                };
            }
            else
            {
                throw new Exception("ParseError");
            }
        }

        /// <summary>
        /// This takes in a Boolean Op. expects an expression on one end an Operator and then another Expression
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public Node BOOLEAN_OP(Token next)
        {
            Node left = EXPRESSION(next,true);
            Node Operator = OPERATORS(Queue.Next());
            Node right = EXPRESSION(Queue.Next());
            return new Node
            {
                TypeName = "Bool Op",
                nodeType = Node.NodeType.BOOLEAN_OP,
                Children = new Node[] { left, Operator, right }
            };
        }

        /// <summary>
        /// An expression is ether made up of an VAR,Constent or another boolean op.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="from_left_boolop"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node EXPRESSION(Token next,bool from_left_boolop = false)
        {
            //For chaining together boolean ops
            if((Queue.Peek().Type == Token.TokenType.OPERATIORS) && (!from_left_boolop))
            {
                return BOOLEAN_OP(next);
            }
            //Expressions can be intergers
            else if(next.Type == Token.TokenType.INTERGER)
            {
                return INTERGER(next);
            }
            else if (next.Type == Token.TokenType.STRING)
            {
                return STRING(next);
            }
            //Or a var
            else if(next.Type == Token.TokenType.VAR)
            {
                return VAR(next);
            }
            else
            {
                throw new Exception("parse exception");
            }
        }

        /// <summary>
        /// A Decloration is the type var name pattern
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node DECLARATION(Token next)
        {
            if(next.Type != Token.TokenType.TYPENAME)
            {
                throw new Exception("Parse Exception");
            }
            return new Node
            {
                TypeName = "DECLARATION",
                Value = next.value,
                nodeType = Node.NodeType.DECLARATION,
                Children = new Node[] { VAR(Queue.Next()) }
            };
        }

        /// <summary>
        /// An assignment is (VAR|DECLERATION) = expression 
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        public Node ASSIGNMENT(Token next)
        {
            Node left = new Node();
            if(next.Type == Token.TokenType.VAR)
            {
                left = VAR(next);
            }
            else if (next.Type == Token.TokenType.TYPENAME)
            {
                left = DECLARATION(next);
            }
            Node OP = ASSIGNMENT_OPERATORS(Queue.Next());
            Node right = EXPRESSION(Queue.Next());
            return new Node
            {
                TypeName = "ASSIGNMENT",
                nodeType = Node.NodeType.ASSIGNMENT,
                Children = new Node[] { left, OP, right }
            };
        }

        public Node PRINT_STATEMENT(Token next)
        {
            if(Queue.Next().Type != Token.TokenType.LEFT_PAR)
            {
                throw new Exception("Parse Error");
            }
            var N = new Node
            {
                nodeType = Node.NodeType.PRINT_STATEMENT,
                TypeName = "PRINT_STATEMENT",
                Children = new Node[] { EXPRESSION(Queue.Next()) }
            };
            if(Queue.Next().Type != Token.TokenType.RIGHT_PAR)
            {
                throw new Exception("Parse error");
            }
            return N;
        }
        /// <summary>
        /// A code block is a collection of Assignments seperated by ; or If,While statements encapusalted in {}
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node CODE_BLOCK(Token next)
        {
            if(next.Type != Token.TokenType.LEFT_BRACKET)
            {
                throw new Exception("ParseException");
            }
            Node[] block = new Node[0];
            while (true)
            {
                next = Queue.Next();
                if(next.Type == Token.TokenType.RIGHT_BRACKET)
                {
                    return new Node
                    {
                        TypeName = "CODE_BLOCK",
                        nodeType = Node.NodeType.CODE_BLOCK,
                        Children = block
                    };
                }
                else if ((next.Type == Token.TokenType.VAR) || (next.Type == Token.TokenType.TYPENAME))
                {
                    block = block.Append(ASSIGNMENT(next)).ToArray();
                    if(Queue.Next().Type != Token.TokenType.SIMI_COLON)
                    {
                        throw new Exception("Parse error");
                    }
                }
                else if (next.Type == Token.TokenType.PRINT)
                {
                    block = block.Append(PRINT_STATEMENT(next)).ToArray();
                    if(Queue.Next().Type != Token.TokenType.SIMI_COLON)
                    {
                        throw new Exception("Parse error");
                    }
                }
                else if (next.Type == Token.TokenType.IF)
                {
                    block = block.Append(IF(next)).ToArray();
                }
                else if (next.Type == Token.TokenType.WHILE)
                {
                    block = block.Append(WHILE(next)).ToArray();
                }
                else
                {
                    throw new Exception("Parse exception");
                }
            }
            throw new Exception("Parse error missing parenthisis");
        }

        /// <summary>
        /// Looks for a truth statement and a code block
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node IF(Token next)
        {
            if(next.Type != Token.TokenType.IF)
            {
                throw new Exception("Parse Error");
            }
            Node truth_value = TRUTH_VALUE(Queue.Next());
            Node Body = CODE_BLOCK(Queue.Next());
            return new Node
            {
                nodeType = Node.NodeType.IF,
                TypeName = "IF",
                Children = new Node[] { truth_value, Body }
            };
        }

        /// <summary>
        /// Looks for a truth statement and code block
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node WHILE(Token next)
        {
            if(next.Type != Token.TokenType.WHILE)
            {
                throw new Exception("Parse Error");
            }
            Node truth_value = TRUTH_VALUE(Queue.Next());
            Node Body = CODE_BLOCK(Queue.Next());
            return new Node
            {
                nodeType = Node.NodeType.WHILE,
                TypeName = "WHILE",
                Children = new Node[] { truth_value, Body }
            };
        }

        /// <summary>
        /// (Expression)
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Node TRUTH_VALUE(Token next)
        {
            if(next.Type != Token.TokenType.LEFT_PAR)
            {
                throw new Exception("Parse exception");
            }
            Node truth_expression = EXPRESSION(Queue.Next());
            if(Queue.Next().Type != Token.TokenType.RIGHT_PAR)
            {
                throw new Exception("Parse exception");
            }
            return truth_expression;
        }
    };
}
