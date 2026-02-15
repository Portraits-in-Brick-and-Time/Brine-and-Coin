using Silverfly;
using Silverfly.Lexing.IgnoreMatcher.Comments;
using Silverfly.Parselets;

namespace ObjectModel.Language;

public class LanguageParser : Parser
{
    protected override void InitLexer(LexerConfig lexer)
    {
        lexer.AddKeywords("let", "if", "then", "else", "enum", "func");

        lexer.IgnoreWhitespace();
        lexer.MatchNumber(allowHex: false, allowBin: false);
        lexer.MatchString("\"", "\"");

        lexer.Ignore(new SingleLineCommentIgnoreMatcher("//"));
        lexer.Ignore(new MultiLineCommentIgnoreMatcher("/*", "*/"));
    }

    protected override void InitParser(ParserDefinition parser)
    {
        parser.AddArithmeticOperators();
        parser.AddCommonLiterals();

        parser.Register("(", new CallParselet(parser.PrecedenceLevels.GetPrecedence("Call")));
        parser.Register(PredefinedSymbols.Name, new NameParselet());

        parser.Block(PredefinedSymbols.SOF, PredefinedSymbols.EOF, PredefinedSymbols.EOL);
    }
}