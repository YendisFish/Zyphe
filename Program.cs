using System;
using Zyphe;

string fle = File.ReadAllText("./Mockups/hello.zp");

Token[] toks = Lexer.Tokenize(fle);
toks = Lexer.RemoveWhitespace(toks);

foreach (var token in toks)
{
    Logger.Log(token.type);
    Logger.Log(token.keyword);
    Logger.Log(token.value);
    Logger.Log("--------------------------------------");
}

