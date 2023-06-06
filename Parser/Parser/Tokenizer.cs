using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    internal class Tokenizer
    {
    }

    internal class TokenMachine
    {
        /// <summary>
        /// Chars that are in symbols
        /// </summary>
        static readonly char[] SymbolChars = { ';','(',')','{','}' };
        /// <summary>
        /// CHars that are in operators
        /// </summary>
        static readonly char[] OperatorChars = { '+', '-', '*', '/', '=','!','<','>' };
        /// <summary>
        /// Keywords that arnt regular identifiers
        /// </summary>
        static readonly string[] KeyWords = { "int", "double","if","while","print" };
        /// <summary>
        /// enum that define what state the state machine is in
        /// </summary>
        enum states { inDigits, inWhitespace, inAlpha, inSymbol, inOperator, inInvalid };
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
        TokenQueu tokens;
        /// <summary>
        /// value of the curent token
        /// </summary>
        string tokenval;
        /// <summary>
        /// Generic constructor
        /// </summary>
        /// <param name="value"></param>
        /// 

        Dictionary<string, Token.TokenType> SymbolLookup;
        Dictionary<string, Token.TokenType> OperatorLookup;
        Dictionary<string, Token.TokenType> KeyWordLookup;
        public TokenMachine(string value)
        {
            tokens = new TokenQueu();
            chars = new Queue<char>();
            tokenval = "";
            foreach (char val in value)
            {
                chars.Enqueue(val);
            }
            SymbolLookup = new Dictionary<string, Token.TokenType>()
            {
                {"(",Token.TokenType.LEFT_PAR },
                {")",Token.TokenType.RIGHT_PAR },
                {"{",Token.TokenType.LEFT_BRACKET },
                {"}",Token.TokenType.RIGHT_BRACKET },
                {";",Token.TokenType.SIMI_COLON }
            };
            OperatorLookup = new Dictionary<string, Token.TokenType>()
            {
                {"+",Token.TokenType.OPERATIORS },
                {"-",Token.TokenType.OPERATIORS },
                {"*",Token.TokenType.OPERATIORS },
                {"=",Token.TokenType.ASSIGNMENT_OPERATOR },
                {"+=",Token.TokenType.ASSIGNMENT_OPERATOR },
                {"-=",Token.TokenType.ASSIGNMENT_OPERATOR },
                {"*=",Token.TokenType.ASSIGNMENT_OPERATOR },
                {"==",Token.TokenType.OPERATIORS },
                {"<",Token.TokenType.OPERATIORS },
                {">",Token.TokenType.OPERATIORS },
                {"<=",Token.TokenType.OPERATIORS },
                {">=",Token.TokenType.OPERATIORS},
                {"!=",Token.TokenType.OPERATIORS }
            };
            KeyWordLookup = new Dictionary<string, Token.TokenType>()
            {
                {"if",Token.TokenType.IF },
                {"while",Token.TokenType.WHILE },
                {"int",Token.TokenType.TYPENAME },
                {"print",Token.TokenType.PRINT }
            };
        }

        /// <summary>
        /// Dequeus a char then runs in through the state machine
        /// </summary>
        /// <returns></returns>
        public TokenQueu run()
        {
            char first = chars.Dequeue();
            state = GetState(first);
            while (chars.Count != 0)
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
                    AddTokenAndReset(Token.TokenType.INTERGER, state);
                    break;
                case states.inSymbol:
                    //switchSym(state);
                    break;
                case states.inOperator:
                    switchOp(state);
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
        void inDigit(char value)
        {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inDigits:
                    tokenval += value;
                    break;
                case states.inAlpha:
                    //If where in a digit state then takes in a alpha char it is then invalid
                    throw new Exception("Token Exception");
                    break;
                default:
                    AddTokenAndReset(Token.TokenType.INTERGER, tokenstate);
                    inGeneric(value);
                    break;
            }
        }
        /// <summary>
        /// method that runs while in white space
        /// </summary>
        /// <param name="value"></param>
        void inWhitespace(char value)
        {
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
        void inAlpha(char value)
        {
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
        void inSymbol(char value)
        {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inSymbol:
                    tokenval += value;
                    switchSym(tokenstate);
                    break;
                default:
                    state = tokenstate;
                    inGeneric(value);
                    break;
            }
        }

        /// <summary>
        /// consumes chars while in the operator state
        /// </summary>
        /// <param name="value"></param>
        void inOperator(char value)
        {
            states tokenstate = GetState(value);
            switch (tokenstate)
            {
                case states.inOperator:
                    tokenval += value;
                    break;
                default:
                    switchOp(tokenstate);
                    inGeneric(value);
                    break;
            }
        }

        /// <summary>
        /// Cosumes chars while in the value state
        /// </summary>
        /// <param name="value"></param>
        void inInvalid(char value)
        {
            throw new Exception("Token exception");
        }

        /// <summary>
        /// Adds a token to the token list and resets the token value
        /// </summary>
        /// <param name="AddingType"></param>
        /// <param name="ToState"></param>
        void AddTokenAndReset(Token.TokenType AddingType, states ToState)
        {
            tokens.EnQueu(new Token { Type = AddingType, value = tokenval });
            tokenval = "";
            state = ToState;
        }


        /// <summary>
        /// for when switching from the alpha method. Classifies into keyword and identifys
        /// </summary>
        /// <param name="ToState"></param>
        void switchAlpha(states ToState)
        {
            if (KeyWords.Contains(tokenval))
            {
                AddTokenAndReset(KeyWordLookup[tokenval], ToState);
            }
            else
            {
                AddTokenAndReset(Token.TokenType.VAR, ToState);
            }
        }

        void switchOp(states ToState) {
            AddTokenAndReset(OperatorLookup[tokenval], ToState);
        }

        void switchSym(states ToState) {
            AddTokenAndReset(SymbolLookup[tokenval], ToState);
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
