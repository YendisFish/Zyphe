using System;
using Newtonsoft.Json;
using Zyphe;
using Zyphe.Parser;

string fle = File.ReadAllText("./Mockups/hello.zp");

bool debug = args.Contains("--debug");

Token[] toks = Lexer.Tokenize(fle);
toks = Lexer.RemoveWhitespace(toks);

if(debug)
{
    foreach (var token in toks)
    {
        Logger.Log(token.type);
        Logger.Log(token.keyword);
        Logger.Log(token.value);
        Logger.Log("--------------------------------------");
    }
}

Parser parser = new Parser(toks);
parser.Parse();

if(debug)
{
    var settings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
    
    string json = JsonConvert.SerializeObject(parser.ast, Formatting.Indented, settings);
    
    Console.WriteLine(json);
}

Console.WriteLine("Done Compiling!");