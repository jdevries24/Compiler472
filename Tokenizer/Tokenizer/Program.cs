// See https://aka.ms/new-console-template for more information
using Tokenizer;

void dumpToken(Token T)
{
    Console.WriteLine($"Type:{T.type.ToString()}, Value:{T.value}");
}
Console.Write("Input string:");
var token = new TokenMachine(Console.ReadLine());
var tokens = token.run();
foreach(var T in tokens)
{
    dumpToken(T);
}

