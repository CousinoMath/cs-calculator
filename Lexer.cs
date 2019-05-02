using System;
using System.Collections.Generic;

class Lexer {
    List<Token> tokens = new List<Token>();
    int start = 0;
    int current = 0;
    int length = 0;
    string source;
    List<string> constants = new List<string> { "pi", "e" };
    List<string> functions = new List<string> { "acos", "asin", "atan",
        "cos", "exp", "log", "sin", "tan" };

    public Lexer(string source) {
        this.source = source.Trim();
        this.length = this.source.Length;
    }

    public Result<List<Token>, string> lex() {
        while (this.current < this.length) {
            this.SkipWhitespace();
            this.start = this.current;
            Result<Token, string> tokenResult = this.NextToken();
            if (tokenResult.IsErr) {
                return new Err<List<Token>, string>(tokenResult.Error);
            }
            this.tokens.Add(tokenResult.Value);
        }
        this.tokens.Add(new Token(TokenType.EOI, "♣"));
        return new Ok<List<Token>, string>(this.tokens);
    }

    private void SkipWhitespace() {
        while (this.current < this.length &&
                Char.IsWhiteSpace(this.source[this.current])) {
            this.Advance();
        }
    }

    private void Advance() {
        if (this.current < this.length) {
            this.current += 1;
        }
    }

    private Result<Token, string> NextToken() {
        char initial = this.source[this.start];
        this.Advance();
        if (Char.IsDigit(initial) || initial == '.') {
            return this.LexNumber();
        }
        if (Char.IsLetter(initial)) {
            return this.LexIdentifier();
        }
        switch(initial) {
            case '+':
                return new Ok<Token, string>(new Token(TokenType.Plus, "+"));
            case '-':
                return new Ok<Token, string>(new Token(TokenType.Dash, "-"));
            case '*':
                return new Ok<Token, string>(new Token(TokenType.Star, "*"));
            case '/':
                return new Ok<Token, string>(new Token(TokenType.Slash, "/"));
            case '^':
                return new Ok<Token, string>(new Token(TokenType.Caret, "^"));
            case '(':
                return new Ok<Token, string>(new Token(TokenType.LParen, "("));
            case ')':
                return new Ok<Token, string>(new Token(TokenType.RParen, ")"));
            case '=':
                return new Ok<Token, string>(new Token(TokenType.Equals, "="));
            default:
                return new Err<Token, string>(String.Format("Unrecognized token {0}", initial));
        }
    }

    private Result<Token, String> LexNumber() {
        while (this.current < this.length &&
                (Char.IsDigit(this.source[this.current]) ||
                 this.source[this.current] == '.')) {
            this.Advance();
        }
        string lexeme = this.source.Substring(this.start, this.current - this.start);
        if (Double.TryParse(lexeme, out double value)) {
            return new Ok<Token, string>(new Token(TokenType.Number, lexeme, value));
        } else {
            return new Err<Token, string>(String.Format("Could not parse {0} as a number", lexeme));
        }
    }

    private Result<Token, String> LexIdentifier() {
        while (this.current < this.length &&
                Char.IsLetterOrDigit(this.source[this.current])) {
            this.Advance();
        }
        string lexeme = this.source.Substring(this.start, this.current - this.start);
        if (constants.Contains(lexeme)) {
            return new Ok<Token, string>(new Token(TokenType.Constant, lexeme));
        }
        if (functions.Contains(lexeme)) {
            return new Ok<Token, string>(new Token(TokenType.Function, lexeme));
        }
        return new Ok<Token, string>(new Token(TokenType.Variable, lexeme));
    }
}