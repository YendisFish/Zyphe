using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Zyphe;
using Zyphe.Parser;

Stopwatch watch = new();

string fle = File.ReadAllText("./Mockups/hello.zp");

bool debug = args.Contains("--debug");

watch.Start();
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

watch.Stop();

Console.WriteLine("Done Compiling!");
Console.WriteLine($"Time: {watch.Elapsed.TotalSeconds}ms");