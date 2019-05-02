using System.Collections.Generic;

class Parser {
    List<Token> tokens;
    int current = 0;
    int length = 0;

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        this.length = tokens.Count;
    }

    public Result<ASTNode, string> Parse() {
        return this.Assignment();
    }

    private Token Peek(int n) {
        if (this.current + n < this.length && this.current + n >= 0) {
            return this.tokens[this.current + n];
        }
        return this.tokens[this.length - 1];
    }

    private void Advance() {
        if (this.current < this.length) {
            this.current += 1;
        }
    }

    private Result<ASTNode, string> Assignment() {
        Token current = this.Peek(0);
        if (current.Type == TokenType.Variable) {
            Token next = this.Peek(1);
            if (next.Type == TokenType.Equals) {
                this.Advance();
                this.Advance();
                Result<ASTNode, string> expression = this.Expression();
                return expression.Map(expr =>
                    (ASTNode)new ASTAssign(current.ToString(), expr));
            }
        }
        return this.Expression();
    }

    private Result<ASTNode, string> Expression() {
        List<ASTNode> arguments = new List<ASTNode>();
        Result<ASTNode, string> result = this.Factor();
        bool loop = true;
        if (result.IsErr) {
            return result;
        }
        arguments.Add(result.Value);
        while (loop && this.current < this.length) {
            Token current = this.Peek(0);
            switch (current.Type) {
                case TokenType.EOI:
                case TokenType.RParen:
                    loop = false;
                    break;
                case TokenType.Plus:
                    this.Advance();
                    result = this.Factor();
                    if (result.IsErr) {
                        return result;
                    }
                    arguments.Add(result.Value);
                    break;
                case TokenType.Dash:
                    this.Advance();
                    result = this.Factor();
                    if (result.IsErr) {
                        return result;
                    }
                    List<ASTNode> negated = new List<ASTNode> {
                        new ASTNumber(-1.0), result.Value
                    };
                    arguments.Add(new ASTTimes(negated));
                    break;
                default:
                    this.Advance();
                    string message = string.Format("Expected a '+' or '-' but instead got {0}",
                        current.Lexeme);
                    return new Err<ASTNode, string>(message);
            }
        }
        switch (arguments.Count) {
            case 0: return new Ok<ASTNode, string>(new ASTNumber(0.0));
            case 1: return new Ok<ASTNode, string>(arguments[0]);
            default: return new Ok<ASTNode, string>(new ASTPlus(arguments));
        }
    }

    private Result<ASTNode, string> Factor() {
        List<ASTNode> arguments = new List<ASTNode>();
        Result<ASTNode, string> result = this.Exponential();
        if (result.IsErr) {
            return result;
        }
        arguments.Add(result.Value);
        bool loop = true;
        while (loop && this.current < this.length) {
            Token current = this.Peek(0);
            switch (current.Type) {
                case TokenType.Plus:
                case TokenType.Dash:
                case TokenType.RParen:
                case TokenType.EOI:
                    loop = false;
                    break;
                case TokenType.Star:
                    this.Advance();
                    result = this.Exponential();
                    if (result.IsErr) {
                        return result;
                    }
                    arguments.Add(result.Value);
                    break;
                case TokenType.Slash:
                    this.Advance();
                    result = this.Exponential();
                    if (result.IsErr) {
                        return result;
                    }
                    arguments.Add(new ASTPower(result.Value, new ASTNumber(-1.0)));
                    break;
                default:
                    string message = string.Format("Expected '*' or '/' but got {0} instead",
                        current.Lexeme);
                    return new Err<ASTNode, string>(message);
            }

        }
        switch (arguments.Count) {
            case 0: return new Ok<ASTNode, string>(new ASTNumber(1.0));
            case 1: return new Ok<ASTNode, string>(arguments[0]);
            default: return new Ok<ASTNode, string>(new ASTTimes(arguments));
        }
    }

    private Result<ASTNode, string> Exponential() {
        Token current = this.Peek(0);
        bool negate = false;
        if (current.Type == TokenType.Dash) {
            this.Advance();
            negate = true;
        }
        Result<ASTNode, string> baseResult = this.Atom();
        if (baseResult.IsErr) {
            return baseResult;
        }
        ASTNode baseNode = baseResult.Value;
        current = this.Peek(0);
        if (current.Type == TokenType.Caret) {
            this.Advance();
            Result<ASTNode, string> expResult = this.Exponential();
            if (expResult.IsErr) {
                return expResult;
            }
            baseNode = new ASTPower(baseNode, expResult.Value);
        }
        if (negate) {
            baseNode = new ASTTimes(new List<ASTNode> {
                new ASTNumber(-1.0), baseNode
            });
        }
        return new Ok<ASTNode, string>(baseNode);
    }

    private Result<ASTNode, string> Atom() {
        Token current = this.Peek(0);
        switch (current.Type) {
            case TokenType.Number:
                this.Advance();
                return new Ok<ASTNode, string>(new ASTNumber(current.Value.Value));
            case TokenType.Constant:
                this.Advance();
                return new Ok<ASTNode, string>(new ASTConstant(current.Lexeme));
            case TokenType.Variable:
                this.Advance();
                return new Ok<ASTNode, string>(new ASTVariable(current.Lexeme));
            case TokenType.Function:
                this.Advance();
                Result<ASTNode, string> argResult = this.Atom();
                if (argResult.IsErr) {
                    return argResult;
                }
                ASTNode arg = argResult.Value;
                return new Ok<ASTNode, string>(new ASTFunction(current.Lexeme, arg));
            case TokenType.LParen:
                this.Advance();
                Result<ASTNode, string> exprResult = this.Expression();
                if (this.Peek(0).Type == TokenType.RParen) {
                    return exprResult;
                }
                return new Err<ASTNode, string>("Unbalanced parentheses");
            default:
                string message = string.Format("Expeced a number, variable, constant, or function but got {0} instead",
                    current.Lexeme);
                return new Err<ASTNode, string>(message);
        }
    }
}