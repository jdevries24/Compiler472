// See https://aka.ms/new-console-template for more information
using Parser;

string codeText = File.ReadAllText("example.txt");
//string codeText = @"{
//int a = 7;
//int b = 5;
//int n = 0;
//string xkcd = ""xkcd"";
//int ans = 0;
//while(n < b){
//    ans += a;
//    n += 1;
//}}";
//First Tokenize the input
TokenMachine Tokenizer = new TokenMachine(codeText);
TokenQueu testQ = Tokenizer.run();
//Then init the parser
testQ.Dump();
DescentParser p = new DescentParser(testQ);
//Finaly parse out the tokens
var tree = p.CODE_BLOCK(testQ.Next());
tree.dump();
CodeGenerator gen = new CodeGenerator(tree);
gen.run();