enum TokenType {
    Plus,
    Dash,
    Star,
    Slash,
    Caret,
    Equals,
    LParen,
    RParen,
    Constant,
    Function,
    Variable,
    Number,
    EOI,
}

class Token {
    private TokenType type;
    private string lexeme;
    private double? value;
    public TokenType Type {
        get => this.type;
    }
    public string Lexeme {
        get => this.lexeme;
    }
    public double? Value {
        get => this.value;
    }
    public Token(TokenType type, string lexeme, double value) {
        this.type = type;
        this.lexeme = lexeme;
        this.value = value;
    }

    public Token(TokenType type, string lexeme) {
        this.type = type;
        this.lexeme = lexeme;
        this.value = null;
    }
}