using System;
using System.Collections.Generic;

namespace helloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            bool cont = true;
            Dictionary<string, double> memory = new Dictionary<string, double>();
            while (cont) {
                Console.Write("> ");
                string input = Console.ReadLine();
                cont = input != null && input != "";
                if (cont) {
                    Console.WriteLine("'{0}' of length {1}", input, input.Length);
                    Lexer lexer = new Lexer(input);
                    Result<List<Token>, string> lexResult = lexer.lex();
                    if (lexResult.IsErr) {
                        Console.WriteLine(lexResult.Error);
                        continue;
                    }
                    List<Token> tokens = lexResult.Value;
                    foreach(Token token in tokens) {
                        Console.Write(token.Lexeme);
                    }
                    Console.WriteLine();
                    Parser parser = new Parser(tokens);
                    Result<ASTNode, string> parseResult = parser.Parse();
                    if (parseResult.IsErr) {
                        Console.WriteLine(parseResult.Error);
                        continue;
                    }
                    ASTNode node = parseResult.Value;
                    Console.WriteLine(node.ToString());
                    Console.WriteLine(node.evaluate(memory));
                }
            }
        }
    }
}
