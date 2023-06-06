using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokenizer
{
    internal class Token
    {
        public enum TokenType { keyword, op, constent, symbol, identifer, invaled,Alpha };
        public TokenType type;
        public string value;
    }
    internal class TokenMachine
    {
        /// <summary>
        /// Chars that are in symbols
        /// </summary>
        static readonly char[] SymbolChars = { ';' };
        /// <summary>
        /// CHars that are in operators
        /// </summary>
        static readonly char[] OperatorChars = { '+','-','*','/','='};
        /// <summary>
        /// Keywords that arnt regular identifiers
        /// </summary>
        static readonly string[] Keyword = { "int", "double" };
        /// <summary>
        /// enum that define what state the state machine is in
        /// </summary>
        enum states { inDigits, inWhitespace, inAlpha,inSymbol,inOperator,inInvalid };
        /// <summary>
        /// holds the state the state machine is in
        /// </summary>
        states state;
        /// <summary>
        /// Queu of chars to be read
        /// </summary>
        Queue<char> chars;
        /// <summary>
        /// List of tokens already tokenized
        /// </summary>
        List<Token> tokens;
        /// <summary>
        /// value of the curent token
        /// </summary>
        string tokenval;
        /// <summary>
        /// Generic constructor
        /// </summary>
        /// <param name="value"></param>
        public TokenMachine(string value)
        {
            tokens = new List<Token>();
            chars = new Queue<char>();
            tokenval = "";
            foreach(char val in value)
            {
                chars.Enqueue(val);
            }
        }

        /// <summary>
        /// Dequeus a char then runs in through the state machine
        /// </summary>
        /// <returns></returns>
        public List<Token> run()
        {
            char first = chars.Dequeue();
            state = GetState(first);
            while(chars.Count != 0)
            {
                inGeneric(first);
                first = chars.Dequeue();
            }
            inGeneric(first);
            switch (state)
            {
                case states.inAlpha:
                    switchAlpha(state);
                    break;
                case states.inDigits:
                    AddTokenAndReset(Token.TokenType.constent, state);
                    break;
                case states.inSymbol:
                    AddTokenAndReset(Token.TokenType.symbol, state);
                    break;
                case states.inOperator:
                    AddTokenAndReset(Token.TokenType.op, state);
                    break;
                default:
                    break;
            }
            return tokens;
        }

        /// <summary>
        /// Method that runs while in the digit state
        /// </summary>
        /// <param name="value"></param>
        void inDigit(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inDigits:
                    tokenval += value;
                    break;
                case states.inAlpha:
                    //If where in a digit state then takes in a alpha char it is then invalid
                    state = states.inInvalid;
                    inGeneric(value);
                    break;
                default:
                    AddTokenAndReset(Token.TokenType.constent, tokenstate);
                    inGeneric(value);
                    break;
            }
        }
        /// <summary>
        /// method that runs while in white space
        /// </summary>
        /// <param name="value"></param>
        void inWhitespace(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inWhitespace:
                    break;
                default:
                    state = tokenstate;
                    inGeneric(value);
                    break;
            }
        }
        /// <summary>
        /// method that runs while in the alpha state
        /// </summary>
        /// <param name="value"></param>
        void inAlpha(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inAlpha:
                    tokenval += value;
                    break;
                case states.inDigits:
                    tokenval += value;
                    break;
                default:
                    switchAlpha(tokenstate);
                    inGeneric(value);
                    break;
            }
        }

        /// <summary>
        /// Consumes the chars while in the symbol state
        /// </summary>
        /// <param name="value"></param>
        void inSymbol(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inSymbol:
                    tokenval += value;
                    break;
                default:
                    AddTokenAndReset(Token.TokenType.symbol, tokenstate);
                    inGeneric(value);
                    break;
            }
        }

        /// <summary>
        /// consumes chars while in the operator state
        /// </summary>
        /// <param name="value"></param>
        void inOperator(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inOperator:
                    tokenval += value;
                    break;
                default:
                    AddTokenAndReset(Token.TokenType.op, tokenstate);
                    inGeneric(value);
                    break;
            }
        }

        /// <summary>
        /// Cosumes chars while in the value state
        /// </summary>
        /// <param name="value"></param>
        void inInvalid(char value) {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inAlpha:
                    tokenval += value;
                    break;
                case states.inDigits:
                    tokenval += value;
                    break;
                default:
                    AddTokenAndReset(Token.TokenType.invaled, tokenstate);
                    break;
            }
        }

        /// <summary>
        /// Adds a token to the token list and resets the token value
        /// </summary>
        /// <param name="AddingType"></param>
        /// <param name="ToState"></param>
        void AddTokenAndReset(Token.TokenType AddingType,states ToState)
        {
            tokens.Add(new Token { type = AddingType, value = tokenval });
            tokenval = "";
            state = ToState;
        }


        /// <summary>
        /// for when switching from the alpha method. Classifies into keyword and identifys
        /// </summary>
        /// <param name="ToState"></param>
        void switchAlpha(states ToState)
        {
            if (Keyword.Contains(tokenval))
            {
                AddTokenAndReset(Token.TokenType.keyword, ToState);
            }
            else
            {
                AddTokenAndReset(Token.TokenType.identifer, ToState);
            }
        }

        /// <summary>
        /// Finds the state that a char indicates
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        states GetState(char value)
        {
            if (OperatorChars.Contains(value))
            {
                return states.inOperator;
            }
            else if (SymbolChars.Contains(value))
            {
                return states.inSymbol;
            }
            else if (char.IsWhiteSpace(value))
            {
                return states.inWhitespace;
            }
            else if (char.IsDigit(value))
            {
                return states.inDigits;
            }
            else
            {
                return states.inAlpha;
            }
        }

        /// <summary>
        /// Switches to the consumpton method of the machines given state.
        /// </summary>
        /// <param name="value"></param>
        void inGeneric(char value)
        {
            switch (state)
            {
                case states.inWhitespace:
                    inWhitespace(value);
                    break;
                case states.inDigits:
                    inDigit(value);
                    break;
                case states.inSymbol:
                    inSymbol(value);
                    break;
                case states.inOperator:
                    inOperator(value);
                    break;
                case states.inInvalid:
                    inInvalid(value);
                    break;
                case states.inAlpha:
                    inAlpha(value);
                    break;
                default:
                    break;
            }
        }
    }
}
