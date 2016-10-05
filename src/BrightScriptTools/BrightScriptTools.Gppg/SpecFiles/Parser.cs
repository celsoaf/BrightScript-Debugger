// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.0.0.0
// Machine:  CELSO-PC
// DateTime: 05/10/2016 15:11:20
// UserName: Celso
// Input file <SpecFiles\BrightScript.y - 05/10/2016 15:10:54>

// options: babel lines diagnose & report gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using BrightScriptTools.Gppg.GPGen;
using System.Collections;
using BrightScriptTools.GPlex;
using BrightScriptTools.GPlex.Parser;
using BrightScriptTools.Compiler.AST;
using BrightScriptTools.Compiler.AST.Tokens;

namespace BrightScriptTools.Compiler
{
public enum Tokens {error=2,EOF=3,bar=4,dot=5,semi=6,
    star=7,lt=8,gt=9,ltEqual=10,gtEqual=11,notEqual=12,
    comma=13,slash=14,lBrac=15,rBrac=16,lPar=17,rPar=18,
    lBrace=19,rBrace=20,Eol=21,equal=22,plus=23,minus=24,
    questionMark=25,colon=26,bsIdent=27,bsNumber=28,bsStr=29,bsCmnt=30,
    bsFuncs=31,bsType=32,bsAs=33,bsTrue=34,bsFalse=35,bsInvalid=36,
    bsNot=37,bsM=38,bsStop=39,bsReturn=40,bsPrint=41,bsIf=42,
    bsElse=43,bsFor=44,bsTo=45,bsEach=46,bsStep=47,bsIn=48,
    bsWhile=49,bsSub=50,bsFunction=51,bsEnd=52,maxParseToken=53,EOL=54,
    comment=55,errTok=56,repErr=57};

// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.0.0.0")]
public abstract class ScanBase : AbstractScanner<SyntaxNodeOrToken,LexSpan> {
  private LexSpan __yylloc = new LexSpan();
  public override LexSpan yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }

  protected abstract int CurrentSc { get; set; }
  //
  // Override the virtual EolState property if the scanner state is more
  // complicated then a simple copy of the current start state ordinal
  //
  public virtual int EolState { get { return CurrentSc; } set { CurrentSc = value; } }
}

// Interface class for 'colorizing' scanners
public interface IColorScan {
  void SetSource(string source, int offset);
  int GetNext(ref int state, out int start, out int end);
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.0.0.0")]
public class ScanObj {
  public int token;
  public SyntaxNodeOrToken yylval;
  public LexSpan yylloc;
  public ScanObj( int t, SyntaxNodeOrToken val, LexSpan loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.0.0.0")]
public partial class Parser: ShiftReduceParser<SyntaxNodeOrToken, LexSpan>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[97];
  private static State[] states = new State[186];
  private static string[] nonTerms = new string[] {
      "Program", "$accept", "SourceElements", "EolOpt", "SourceElement", "SubDeclaration", 
      "FunctionDeclaration", "ParameterList", "Type", "StatementList", "Parameter", 
      "ParameterTail", "Literal", "Statement", "DebuggerStatement", "PrintStatement", 
      "SequenceExpression", "IterationStatement", "IfStatement", "AssignStatement", 
      "ReturnStatement", "SingleExpression", "Block", "IndexExpression", "MemberExpression", 
      "BinaryExpression", "UnaryExpression", "CallExpression", "LabelledStatementList", 
      "LabelledStatement", "LabelledStatementTail", "LabelSeparator", "FunctionStatement", 
      "Arguments", "MathOperator", "BooleanOperator", "EmptyBlock", "Array", 
      "NullLiteral", "BooleanLiteral", "StringLiteral", "NumericLiteral", "ArrayList", 
      "ArrayTail", };

  static Parser() {
    states[0] = new State(new int[]{21,8,50,-17,51,-17,3,-17},new int[]{-1,1,-3,3,-4,5});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{3,4});
    states[4] = new State(-2);
    states[5] = new State(new int[]{50,11,51,177,3,-3},new int[]{-5,6,-6,10,-7,176});
    states[6] = new State(new int[]{21,8,50,-17,51,-17,3,-17},new int[]{-3,7,-4,5});
    states[7] = new State(-4);
    states[8] = new State(new int[]{21,8,50,-17,51,-17,3,-17,39,-17,41,-17,25,-17,19,-17,27,-17,17,-17,24,-17,37,-17,31,-17,15,-17,36,-17,34,-17,35,-17,29,-17,28,-17,44,-17,49,-17,42,-17,40,-17,52,-17,43,-17,20,-17,16,-17},new int[]{-4,9});
    states[9] = new State(-18);
    states[10] = new State(-5);
    states[11] = new State(new int[]{27,12});
    states[12] = new State(new int[]{17,13});
    states[13] = new State(new int[]{27,89,18,-9},new int[]{-8,14,-11,84});
    states[14] = new State(new int[]{18,15});
    states[15] = new State(new int[]{21,20,52,-17},new int[]{-10,16,-4,19});
    states[16] = new State(new int[]{52,17});
    states[17] = new State(new int[]{50,18});
    states[18] = new State(-8);
    states[19] = new State(-19);
    states[20] = new State(new int[]{21,8,39,-17,41,-17,25,-17,19,-17,27,-17,17,-17,24,-17,37,-17,31,-17,15,-17,36,-17,34,-17,35,-17,29,-17,28,-17,44,-17,49,-17,42,-17,40,-17,52,-17,43,-17},new int[]{-4,21});
    states[21] = new State(new int[]{39,25,41,27,25,135,19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134,44,141,49,156,42,162,40,173,52,-18,43,-18},new int[]{-14,22,-15,24,-16,26,-17,137,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133,-18,140,-19,161,-20,171,-21,172});
    states[22] = new State(new int[]{21,20,52,-17,43,-17},new int[]{-10,23,-4,19});
    states[23] = new State(-20);
    states[24] = new State(-21);
    states[25] = new State(-36);
    states[26] = new State(-22);
    states[27] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,28,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[28] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-37,52,-37,43,-37},new int[]{-35,32,-36,34});
    states[29] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,30,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[30] = new State(new int[]{16,31,15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54},new int[]{-35,32,-36,34});
    states[31] = new State(-69);
    states[32] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,33,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[33] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,5,-66,21,-66,52,-66,43,-66,16,-66,13,-66,18,-66,20,-66,45,-66},new int[]{-35,32,-36,34});
    states[34] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,35,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[35] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,5,-67,21,-67,52,-67,43,-67,16,-67,13,-67,18,-67,20,-67,45,-67},new int[]{-35,32,-36,34});
    states[36] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134,18,-60},new int[]{-34,37,-17,39,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[37] = new State(new int[]{18,38});
    states[38] = new State(-59);
    states[39] = new State(new int[]{15,29,17,36,13,40,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,18,-61},new int[]{-35,32,-36,34});
    states[40] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134,18,-60},new int[]{-34,41,-17,39,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[41] = new State(-62);
    states[42] = new State(new int[]{5,43,15,-39,17,-39,22,-39,23,-39,24,-39,7,-39,14,-39,8,-39,10,-39,9,-39,11,-39,12,-39,21,-39,52,-39,43,-39,16,-39,13,-39,18,-39,20,-39,45,-39});
    states[43] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,44,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[44] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-40,52,-40,43,-40,16,-40,5,-40,13,-40,18,-40,20,-40,45,-40},new int[]{-35,32,-36,34});
    states[45] = new State(-70);
    states[46] = new State(-71);
    states[47] = new State(-72);
    states[48] = new State(-73);
    states[49] = new State(-74);
    states[50] = new State(-75);
    states[51] = new State(-76);
    states[52] = new State(-77);
    states[53] = new State(-78);
    states[54] = new State(-79);
    states[55] = new State(-41);
    states[56] = new State(new int[]{20,59,21,8,27,-17},new int[]{-29,57,-4,60});
    states[57] = new State(new int[]{20,58});
    states[58] = new State(-48);
    states[59] = new State(-91);
    states[60] = new State(new int[]{27,70,20,-49},new int[]{-30,61});
    states[61] = new State(new int[]{21,64,13,69,20,-17},new int[]{-31,62,-4,63,-32,66});
    states[62] = new State(-50);
    states[63] = new State(-51);
    states[64] = new State(new int[]{21,8,20,-17,27,-17,16,-17,19,-17,17,-17,24,-17,37,-17,31,-17,15,-17,36,-17,34,-17,35,-17,29,-17,28,-17},new int[]{-4,65});
    states[65] = new State(new int[]{20,-18,16,-18,27,-56,19,-56,17,-56,24,-56,37,-56,31,-56,15,-56,36,-56,34,-56,35,-56,29,-56,28,-56});
    states[66] = new State(new int[]{27,70},new int[]{-30,67});
    states[67] = new State(new int[]{21,64,13,69,20,-17},new int[]{-31,68,-4,63,-32,66});
    states[68] = new State(-52);
    states[69] = new State(-57);
    states[70] = new State(new int[]{26,71});
    states[71] = new State(new int[]{51,74,19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-33,72,-17,73,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[72] = new State(-53);
    states[73] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-54,13,-54,20,-54},new int[]{-35,32,-36,34});
    states[74] = new State(new int[]{17,75});
    states[75] = new State(new int[]{27,89,18,-9},new int[]{-8,76,-11,84});
    states[76] = new State(new int[]{18,77});
    states[77] = new State(new int[]{33,82,21,-15,52,-15},new int[]{-9,78});
    states[78] = new State(new int[]{21,20,52,-17},new int[]{-10,79,-4,19});
    states[79] = new State(new int[]{52,80});
    states[80] = new State(new int[]{51,81});
    states[81] = new State(-55);
    states[82] = new State(new int[]{32,83});
    states[83] = new State(-16);
    states[84] = new State(new int[]{13,86,18,-11},new int[]{-12,85});
    states[85] = new State(-10);
    states[86] = new State(new int[]{27,89},new int[]{-11,87});
    states[87] = new State(new int[]{13,86,18,-11},new int[]{-12,88});
    states[88] = new State(-12);
    states[89] = new State(new int[]{22,91,33,82,13,-15,18,-15},new int[]{-9,90});
    states[90] = new State(-13);
    states[91] = new State(new int[]{19,95,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-13,92,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[92] = new State(new int[]{33,82,13,-15,18,-15},new int[]{-9,93});
    states[93] = new State(-14);
    states[94] = new State(-80);
    states[95] = new State(new int[]{20,59});
    states[96] = new State(-81);
    states[97] = new State(new int[]{21,8,19,-17,27,-17,17,-17,24,-17,37,-17,31,-17,15,-17,36,-17,34,-17,35,-17,29,-17,28,-17,16,-17},new int[]{-43,98,-4,100});
    states[98] = new State(new int[]{16,99});
    states[99] = new State(-92);
    states[100] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134,16,-93},new int[]{-17,101,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[101] = new State(new int[]{15,29,17,36,21,64,13,69,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,16,-17},new int[]{-44,102,-35,32,-36,34,-4,103,-32,104});
    states[102] = new State(-94);
    states[103] = new State(-95);
    states[104] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,105,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[105] = new State(new int[]{15,29,17,36,21,64,13,69,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,16,-17},new int[]{-44,106,-35,32,-36,34,-4,103,-32,104});
    states[106] = new State(-96);
    states[107] = new State(-42);
    states[108] = new State(-43);
    states[109] = new State(-68);
    states[110] = new State(-44);
    states[111] = new State(-45);
    states[112] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,113,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[113] = new State(new int[]{18,114,15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54},new int[]{-35,32,-36,34});
    states[114] = new State(-63);
    states[115] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-22,116,-23,55,-24,107,-17,117,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[116] = new State(new int[]{5,43,15,-39,17,-39,22,-39,23,-39,24,-39,7,-39,14,-39,8,-39,10,-39,9,-39,11,-39,12,-39,21,-64,52,-64,43,-64,16,-64,13,-64,18,-64,20,-64,45,-64});
    states[117] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54},new int[]{-35,32,-36,34});
    states[118] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-22,119,-23,55,-24,107,-17,117,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[119] = new State(new int[]{5,43,15,-39,17,-39,22,-39,23,-39,24,-39,7,-39,14,-39,8,-39,10,-39,9,-39,11,-39,12,-39,21,-65,52,-65,43,-65,16,-65,13,-65,18,-65,20,-65,45,-65});
    states[120] = new State(-46);
    states[121] = new State(new int[]{17,122});
    states[122] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134,18,-60},new int[]{-34,123,-17,39,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[123] = new State(new int[]{18,124});
    states[124] = new State(-58);
    states[125] = new State(-47);
    states[126] = new State(-82);
    states[127] = new State(-86);
    states[128] = new State(-83);
    states[129] = new State(-87);
    states[130] = new State(-88);
    states[131] = new State(-84);
    states[132] = new State(-89);
    states[133] = new State(-85);
    states[134] = new State(-90);
    states[135] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,136,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[136] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-38,52,-38,43,-38},new int[]{-35,32,-36,34});
    states[137] = new State(new int[]{15,29,17,36,22,138,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,12,54,21,-23,52,-23,43,-23},new int[]{-35,32,-36,34});
    states[138] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,139,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[139] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-28,52,-28,43,-28,45,-28},new int[]{-35,32,-36,34});
    states[140] = new State(-24);
    states[141] = new State(new int[]{46,148,19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-20,142,-17,155,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[142] = new State(new int[]{45,143});
    states[143] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,144,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[144] = new State(new int[]{15,29,17,36,21,20,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,52,-17},new int[]{-10,145,-35,32,-36,34,-4,19});
    states[145] = new State(new int[]{52,146});
    states[146] = new State(new int[]{44,147});
    states[147] = new State(-31);
    states[148] = new State(new int[]{27,149});
    states[149] = new State(new int[]{48,150});
    states[150] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,151,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[151] = new State(new int[]{15,29,17,36,21,20,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,52,-17},new int[]{-10,152,-35,32,-36,34,-4,19});
    states[152] = new State(new int[]{52,153});
    states[153] = new State(new int[]{44,154});
    states[154] = new State(-32);
    states[155] = new State(new int[]{22,138,15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,12,54},new int[]{-35,32,-36,34});
    states[156] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,157,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[157] = new State(new int[]{15,29,17,36,21,20,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,52,-17},new int[]{-10,158,-35,32,-36,34,-4,19});
    states[158] = new State(new int[]{52,159});
    states[159] = new State(new int[]{49,160});
    states[160] = new State(-33);
    states[161] = new State(-25);
    states[162] = new State(new int[]{19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,163,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[163] = new State(new int[]{15,29,17,36,21,20,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,52,-17,43,-17},new int[]{-10,164,-35,32,-36,34,-4,19});
    states[164] = new State(new int[]{52,165,43,167});
    states[165] = new State(new int[]{42,166});
    states[166] = new State(-29);
    states[167] = new State(new int[]{21,20,52,-17},new int[]{-10,168,-4,19});
    states[168] = new State(new int[]{52,169});
    states[169] = new State(new int[]{42,170});
    states[170] = new State(-30);
    states[171] = new State(-26);
    states[172] = new State(-27);
    states[173] = new State(new int[]{21,175,19,56,27,109,17,112,24,115,37,118,31,121,15,97,36,127,34,129,35,130,29,132,28,134},new int[]{-17,174,-22,42,-23,55,-24,107,-25,108,-26,110,-27,111,-28,120,-13,125,-37,94,-38,96,-39,126,-40,128,-41,131,-42,133});
    states[174] = new State(new int[]{15,29,17,36,23,45,24,46,7,47,14,48,8,49,10,50,9,51,11,52,22,53,12,54,21,-34,52,-34,43,-34},new int[]{-35,32,-36,34});
    states[175] = new State(-35);
    states[176] = new State(-6);
    states[177] = new State(new int[]{27,178});
    states[178] = new State(new int[]{17,179});
    states[179] = new State(new int[]{27,89,18,-9},new int[]{-8,180,-11,84});
    states[180] = new State(new int[]{18,181});
    states[181] = new State(new int[]{33,82,21,-15,52,-15},new int[]{-9,182});
    states[182] = new State(new int[]{21,20,52,-17},new int[]{-10,183,-4,19});
    states[183] = new State(new int[]{52,184});
    states[184] = new State(new int[]{51,185});
    states[185] = new State(-7);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-3,3});
    rules[3] = new Rule(-3, new int[]{-4});
    rules[4] = new Rule(-3, new int[]{-4,-5,-3});
    rules[5] = new Rule(-5, new int[]{-6});
    rules[6] = new Rule(-5, new int[]{-7});
    rules[7] = new Rule(-7, new int[]{51,27,17,-8,18,-9,-10,52,51});
    rules[8] = new Rule(-6, new int[]{50,27,17,-8,18,-10,52,50});
    rules[9] = new Rule(-8, new int[]{});
    rules[10] = new Rule(-8, new int[]{-11,-12});
    rules[11] = new Rule(-12, new int[]{});
    rules[12] = new Rule(-12, new int[]{13,-11,-12});
    rules[13] = new Rule(-11, new int[]{27,-9});
    rules[14] = new Rule(-11, new int[]{27,22,-13,-9});
    rules[15] = new Rule(-9, new int[]{});
    rules[16] = new Rule(-9, new int[]{33,32});
    rules[17] = new Rule(-4, new int[]{});
    rules[18] = new Rule(-4, new int[]{21,-4});
    rules[19] = new Rule(-10, new int[]{-4});
    rules[20] = new Rule(-10, new int[]{21,-4,-14,-10});
    rules[21] = new Rule(-14, new int[]{-15});
    rules[22] = new Rule(-14, new int[]{-16});
    rules[23] = new Rule(-14, new int[]{-17});
    rules[24] = new Rule(-14, new int[]{-18});
    rules[25] = new Rule(-14, new int[]{-19});
    rules[26] = new Rule(-14, new int[]{-20});
    rules[27] = new Rule(-14, new int[]{-21});
    rules[28] = new Rule(-20, new int[]{-17,22,-17});
    rules[29] = new Rule(-19, new int[]{42,-17,-10,52,42});
    rules[30] = new Rule(-19, new int[]{42,-17,-10,43,-10,52,42});
    rules[31] = new Rule(-18, new int[]{44,-20,45,-17,-10,52,44});
    rules[32] = new Rule(-18, new int[]{44,46,27,48,-17,-10,52,44});
    rules[33] = new Rule(-18, new int[]{49,-17,-10,52,49});
    rules[34] = new Rule(-21, new int[]{40,-17});
    rules[35] = new Rule(-21, new int[]{40,21});
    rules[36] = new Rule(-15, new int[]{39});
    rules[37] = new Rule(-16, new int[]{41,-17});
    rules[38] = new Rule(-16, new int[]{25,-17});
    rules[39] = new Rule(-17, new int[]{-22});
    rules[40] = new Rule(-17, new int[]{-22,5,-17});
    rules[41] = new Rule(-22, new int[]{-23});
    rules[42] = new Rule(-22, new int[]{-24});
    rules[43] = new Rule(-22, new int[]{-25});
    rules[44] = new Rule(-22, new int[]{-26});
    rules[45] = new Rule(-22, new int[]{-27});
    rules[46] = new Rule(-22, new int[]{-28});
    rules[47] = new Rule(-22, new int[]{-13});
    rules[48] = new Rule(-23, new int[]{19,-29,20});
    rules[49] = new Rule(-29, new int[]{-4});
    rules[50] = new Rule(-29, new int[]{-4,-30,-31});
    rules[51] = new Rule(-31, new int[]{-4});
    rules[52] = new Rule(-31, new int[]{-32,-30,-31});
    rules[53] = new Rule(-30, new int[]{27,26,-33});
    rules[54] = new Rule(-30, new int[]{27,26,-17});
    rules[55] = new Rule(-33, new int[]{51,17,-8,18,-9,-10,52,51});
    rules[56] = new Rule(-32, new int[]{21,-4});
    rules[57] = new Rule(-32, new int[]{13});
    rules[58] = new Rule(-28, new int[]{31,17,-34,18});
    rules[59] = new Rule(-28, new int[]{-17,17,-34,18});
    rules[60] = new Rule(-34, new int[]{});
    rules[61] = new Rule(-34, new int[]{-17});
    rules[62] = new Rule(-34, new int[]{-17,13,-34});
    rules[63] = new Rule(-27, new int[]{17,-17,18});
    rules[64] = new Rule(-27, new int[]{24,-22});
    rules[65] = new Rule(-27, new int[]{37,-22});
    rules[66] = new Rule(-26, new int[]{-17,-35,-17});
    rules[67] = new Rule(-26, new int[]{-17,-36,-17});
    rules[68] = new Rule(-25, new int[]{27});
    rules[69] = new Rule(-24, new int[]{-17,15,-17,16});
    rules[70] = new Rule(-35, new int[]{23});
    rules[71] = new Rule(-35, new int[]{24});
    rules[72] = new Rule(-35, new int[]{7});
    rules[73] = new Rule(-35, new int[]{14});
    rules[74] = new Rule(-36, new int[]{8});
    rules[75] = new Rule(-36, new int[]{10});
    rules[76] = new Rule(-36, new int[]{9});
    rules[77] = new Rule(-36, new int[]{11});
    rules[78] = new Rule(-36, new int[]{22});
    rules[79] = new Rule(-36, new int[]{12});
    rules[80] = new Rule(-13, new int[]{-37});
    rules[81] = new Rule(-13, new int[]{-38});
    rules[82] = new Rule(-13, new int[]{-39});
    rules[83] = new Rule(-13, new int[]{-40});
    rules[84] = new Rule(-13, new int[]{-41});
    rules[85] = new Rule(-13, new int[]{-42});
    rules[86] = new Rule(-39, new int[]{36});
    rules[87] = new Rule(-40, new int[]{34});
    rules[88] = new Rule(-40, new int[]{35});
    rules[89] = new Rule(-41, new int[]{29});
    rules[90] = new Rule(-42, new int[]{28});
    rules[91] = new Rule(-37, new int[]{19,20});
    rules[92] = new Rule(-38, new int[]{15,-43,16});
    rules[93] = new Rule(-43, new int[]{-4});
    rules[94] = new Rule(-43, new int[]{-4,-17,-44});
    rules[95] = new Rule(-44, new int[]{-4});
    rules[96] = new Rule(-44, new int[]{-32,-17,-44});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 9: // ParameterList -> /* empty */
#line 54 "SpecFiles\BrightScript.y"
                   { CurrentSemanticValue = BuildParameterListNode(); }
#line default
        break;
      case 10: // ParameterList -> Parameter, ParameterTail
#line 55 "SpecFiles\BrightScript.y"
                            { CurrentSemanticValue = BuildParameterListNode(ValueStack[ValueStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 11: // ParameterTail -> /* empty */
#line 59 "SpecFiles\BrightScript.y"
                   { CurrentSemanticValue = BuildParameterListNode(); }
#line default
        break;
      case 12: // ParameterTail -> comma, Parameter, ParameterTail
#line 60 "SpecFiles\BrightScript.y"
                                 { CurrentSemanticValue = BuildParameterListNode(LocationStack[LocationStack.Depth-3], ValueStack[ValueStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 13: // Parameter -> bsIdent, Type
#line 64 "SpecFiles\BrightScript.y"
                    { CurrentSemanticValue = BuildParameterNode(LocationStack[LocationStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 14: // Parameter -> bsIdent, equal, Literal, Type
#line 65 "SpecFiles\BrightScript.y"
                              { CurrentSemanticValue = BuildParameterNode(LocationStack[LocationStack.Depth-4], LocationStack[LocationStack.Depth-3], ValueStack[ValueStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 16: // Type -> bsAs, bsType
#line 70 "SpecFiles\BrightScript.y"
                   { CurrentSemanticValue = BuildTypeNode(LocationStack[LocationStack.Depth-2], LocationStack[LocationStack.Depth-1]); }
#line default
        break;
      case 39: // SequenceExpression -> SingleExpression
#line 123 "SpecFiles\BrightScript.y"
                          { CurrentSemanticValue = BuildSequenceExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 40: // SequenceExpression -> SingleExpression, dot, SequenceExpression
#line 124 "SpecFiles\BrightScript.y"
                                           { CurrentSemanticValue = BuildSequenceExpressionNode(ValueStack[ValueStack.Depth-3], LocationStack[LocationStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 41: // SingleExpression -> Block
#line 128 "SpecFiles\BrightScript.y"
               { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 42: // SingleExpression -> IndexExpression
#line 129 "SpecFiles\BrightScript.y"
                      { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 43: // SingleExpression -> MemberExpression
#line 130 "SpecFiles\BrightScript.y"
                       { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 44: // SingleExpression -> BinaryExpression
#line 131 "SpecFiles\BrightScript.y"
                       { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 45: // SingleExpression -> UnaryExpression
#line 132 "SpecFiles\BrightScript.y"
                      { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 46: // SingleExpression -> CallExpression
#line 133 "SpecFiles\BrightScript.y"
                     { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 47: // SingleExpression -> Literal
#line 134 "SpecFiles\BrightScript.y"
                { CurrentSemanticValue = BuildSingleExpressionNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 60: // Arguments -> /* empty */
#line 171 "SpecFiles\BrightScript.y"
                     { CurrentSemanticValue = BuildArgumentsNode(); }
#line default
        break;
      case 61: // Arguments -> SequenceExpression
#line 172 "SpecFiles\BrightScript.y"
                          { CurrentSemanticValue = BuildArgumentsNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 62: // Arguments -> SequenceExpression, comma, Arguments
#line 173 "SpecFiles\BrightScript.y"
                                      { CurrentSemanticValue = BuildArgumentsNode(ValueStack[ValueStack.Depth-3], LocationStack[LocationStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 63: // UnaryExpression -> lPar, SequenceExpression, rPar
#line 177 "SpecFiles\BrightScript.y"
                                { CurrentSemanticValue = BuildUnaryExpressionNode(LocationStack[LocationStack.Depth-3], ValueStack[ValueStack.Depth-2], LocationStack[LocationStack.Depth-1]); }
#line default
        break;
      case 64: // UnaryExpression -> minus, SingleExpression
#line 178 "SpecFiles\BrightScript.y"
                           { CurrentSemanticValue = BuildUnaryExpressionNode(LocationStack[LocationStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 65: // UnaryExpression -> bsNot, SingleExpression
#line 179 "SpecFiles\BrightScript.y"
                           { CurrentSemanticValue = BuildUnaryExpressionNode(LocationStack[LocationStack.Depth-2], ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 68: // MemberExpression -> bsIdent
#line 188 "SpecFiles\BrightScript.y"
             { CurrentSemanticValue = BuildMemberExpressionNode(CurrentLocationSpan); }
#line default
        break;
      case 70: // MathOperator -> plus
#line 196 "SpecFiles\BrightScript.y"
           { CurrentSemanticValue = BuildMathOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 71: // MathOperator -> minus
#line 197 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildMathOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 72: // MathOperator -> star
#line 198 "SpecFiles\BrightScript.y"
           { CurrentSemanticValue = BuildMathOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 73: // MathOperator -> slash
#line 199 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildMathOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 74: // BooleanOperator -> lt
#line 203 "SpecFiles\BrightScript.y"
         { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 75: // BooleanOperator -> ltEqual
#line 204 "SpecFiles\BrightScript.y"
             { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 76: // BooleanOperator -> gt
#line 205 "SpecFiles\BrightScript.y"
         { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 77: // BooleanOperator -> gtEqual
#line 206 "SpecFiles\BrightScript.y"
             { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 78: // BooleanOperator -> equal
#line 207 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 79: // BooleanOperator -> notEqual
#line 208 "SpecFiles\BrightScript.y"
              { CurrentSemanticValue = BuildBooleanOperatorNode(CurrentLocationSpan); }
#line default
        break;
      case 80: // Literal -> EmptyBlock
#line 212 "SpecFiles\BrightScript.y"
               { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 81: // Literal -> Array
#line 213 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 82: // Literal -> NullLiteral
#line 214 "SpecFiles\BrightScript.y"
                { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 83: // Literal -> BooleanLiteral
#line 215 "SpecFiles\BrightScript.y"
                  { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 84: // Literal -> StringLiteral
#line 216 "SpecFiles\BrightScript.y"
                  { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 85: // Literal -> NumericLiteral
#line 217 "SpecFiles\BrightScript.y"
                  { CurrentSemanticValue = BuildLiteralNode(ValueStack[ValueStack.Depth-1]); }
#line default
        break;
      case 86: // NullLiteral -> bsInvalid
#line 221 "SpecFiles\BrightScript.y"
               { CurrentSemanticValue = BuildInvalidNode(CurrentLocationSpan); }
#line default
        break;
      case 87: // BooleanLiteral -> bsTrue
#line 225 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildBooleanNode(CurrentLocationSpan, true); }
#line default
        break;
      case 88: // BooleanLiteral -> bsFalse
#line 226 "SpecFiles\BrightScript.y"
             { CurrentSemanticValue = BuildBooleanNode(CurrentLocationSpan, false); }
#line default
        break;
      case 89: // StringLiteral -> bsStr
#line 230 "SpecFiles\BrightScript.y"
            { CurrentSemanticValue = BuildStringNode(CurrentLocationSpan); }
#line default
        break;
      case 90: // NumericLiteral -> bsNumber
#line 234 "SpecFiles\BrightScript.y"
              { CurrentSemanticValue = BuildNumberNode(CurrentLocationSpan); }
#line default
        break;
      case 91: // EmptyBlock -> lBrace, rBrace
#line 238 "SpecFiles\BrightScript.y"
                  { CurrentSemanticValue = BuildEmptyBlock(LocationStack[LocationStack.Depth-2], LocationStack[LocationStack.Depth-1]); }
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 255 "SpecFiles\BrightScript.y"
 #line default
}
}
