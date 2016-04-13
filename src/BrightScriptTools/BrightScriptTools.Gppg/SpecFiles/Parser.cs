// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.0.0.0
// Machine:  OSTLT0248323
// DateTime: 13/04/2016 21:08:26
// UserName: CFE05
// Input file <SpecFiles\BrightScript.y - 13/04/2016 21:08:19>

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

namespace BrightScriptTools.Compiler
{
public enum Tokens {error=2,EOF=3,bar=4,dot=5,semi=6,
    star=7,lt=8,gt=9,ltEqual=10,gtEqual=11,notEqual=12,
    comma=13,slash=14,lBrac=15,rBrac=16,lPar=17,rPar=18,
    lBrace=19,rBrace=20,Eol=21,equal=22,plus=23,minus=24,
    questionMark=25,bsIdent=26,bsNumber=27,bsStr=28,bsCmnt=29,bsFuncs=30,
    bsType=31,bsAs=32,bsTrue=33,bsFalse=34,bsInvalid=35,bsNot=36,
    bsM=37,bsStop=38,bsReturn=39,bsPrint=40,bsIf=41,bsElse=42,
    bsFor=43,bsTo=44,bsEach=45,bsStep=46,bsIn=47,bsWhile=48,
    bsSub=49,bsFunction=50,bsEnd=51,maxParseToken=52,EOL=53,comment=54,
    errTok=55,repErr=56};

// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.0.0.0")]
public abstract class ScanBase : AbstractScanner<int,LexSpan> {
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
  public int yylval;
  public LexSpan yylloc;
  public ScanObj( int t, int val, LexSpan loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.0.0.0")]
public partial class Parser: ShiftReduceParser<int, LexSpan>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[75];
  private static State[] states = new State[145];
  private static string[] nonTerms = new string[] {
      "Program", "$accept", "SourceElements", "EolOpt", "SourceElement", "SubDeclaration", 
      "FunctionDeclaration", "ParameterList", "Type", "StatementList", "Parameter", 
      "ParameterTail", "Literal", "Statement", "AssignStatement", "IfStatement", 
      "IterationStatement", "ReturnStatement", "DebuggerStatement", "PrintStatement", 
      "SingleExpression", "MemberExpression", "BooleanExpression", "UnaryExpression", 
      "CallExpression", "BinaryExpression", "BooleanOperator", "Arguments", "NullLiteral", 
      "BooleanLiteral", "StringLiteral", "NumericLiteral", };

  static Parser() {
    states[0] = new State(new int[]{21,8,49,-18,50,-18,3,-18},new int[]{-1,1,-3,3,-4,5});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{3,4});
    states[4] = new State(-2);
    states[5] = new State(new int[]{49,11,50,136,3,-4},new int[]{-5,6,-6,10,-7,135});
    states[6] = new State(new int[]{21,8,49,-18,50,-18,3,-18},new int[]{-3,7,-4,5});
    states[7] = new State(-3);
    states[8] = new State(new int[]{21,8,49,-18,50,-18,3,-18,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18,42,-18},new int[]{-4,9});
    states[9] = new State(-17);
    states[10] = new State(-5);
    states[11] = new State(new int[]{26,12});
    states[12] = new State(new int[]{17,13});
    states[13] = new State(new int[]{26,128,18,-10},new int[]{-8,14,-11,123});
    states[14] = new State(new int[]{18,15});
    states[15] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,16,-4,19});
    states[16] = new State(new int[]{51,17});
    states[17] = new State(new int[]{49,18});
    states[18] = new State(-8);
    states[19] = new State(new int[]{26,39,41,69,43,92,48,106,39,112,38,116,40,118,25,120,17,27,24,30,30,45,35,52,33,54,34,55,28,57,27,59,51,-20,42,-20},new int[]{-14,20,-15,22,-22,23,-16,68,-17,91,-18,111,-19,115,-20,117,-21,122,-24,26,-25,32,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[20] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18,42,-18},new int[]{-10,21,-4,19});
    states[21] = new State(-19);
    states[22] = new State(-21);
    states[23] = new State(new int[]{22,24,17,34,23,60,24,62,7,64,14,66});
    states[24] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,25,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[25] = new State(-28);
    states[26] = new State(-39);
    states[27] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,28,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[28] = new State(new int[]{18,29});
    states[29] = new State(-51);
    states[30] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,31,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[31] = new State(-52);
    states[32] = new State(-40);
    states[33] = new State(new int[]{17,34,23,60,24,62,7,64,14,66});
    states[34] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59,18,-50},new int[]{-28,35,-21,37,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[35] = new State(new int[]{18,36});
    states[36] = new State(-47);
    states[37] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59,18,-50},new int[]{-28,38,-21,37,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[38] = new State(-49);
    states[39] = new State(new int[]{5,40,15,42,22,-57,17,-57,23,-57,24,-57,7,-57,14,-57});
    states[40] = new State(new int[]{26,39},new int[]{-22,41});
    states[41] = new State(-58);
    states[42] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,43,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[43] = new State(new int[]{16,44});
    states[44] = new State(-59);
    states[45] = new State(new int[]{17,46});
    states[46] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59,18,-50},new int[]{-28,47,-21,37,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[47] = new State(new int[]{18,48});
    states[48] = new State(-48);
    states[49] = new State(-41);
    states[50] = new State(-42);
    states[51] = new State(-66);
    states[52] = new State(-70);
    states[53] = new State(-67);
    states[54] = new State(-71);
    states[55] = new State(-72);
    states[56] = new State(-68);
    states[57] = new State(-73);
    states[58] = new State(-69);
    states[59] = new State(-74);
    states[60] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,61,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[61] = new State(-53);
    states[62] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,63,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[63] = new State(-54);
    states[64] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,65,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[65] = new State(-55);
    states[66] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,67,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[67] = new State(-56);
    states[68] = new State(-22);
    states[69] = new State(new int[]{33,78,34,79,36,80,17,27,24,30,26,39,30,45,35,52,28,57,27,59},new int[]{-23,70,-21,82,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[70] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18,42,-18},new int[]{-10,71,-4,19});
    states[71] = new State(new int[]{51,72,42,74});
    states[72] = new State(new int[]{41,73});
    states[73] = new State(-29);
    states[74] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,75,-4,19});
    states[75] = new State(new int[]{51,76});
    states[76] = new State(new int[]{41,77});
    states[77] = new State(-30);
    states[78] = new State(new int[]{21,-43,51,-43,42,-43,26,-43,41,-43,43,-43,48,-43,39,-43,38,-43,40,-43,25,-43,17,-43,24,-43,30,-43,35,-43,33,-43,34,-43,28,-43,27,-43,8,-71,10,-71,9,-71,11,-71,22,-71,12,-71});
    states[79] = new State(new int[]{21,-44,51,-44,42,-44,26,-44,41,-44,43,-44,48,-44,39,-44,38,-44,40,-44,25,-44,17,-44,24,-44,30,-44,35,-44,33,-44,34,-44,28,-44,27,-44,8,-72,10,-72,9,-72,11,-72,22,-72,12,-72});
    states[80] = new State(new int[]{33,78,34,79,36,80,17,27,24,30,26,39,30,45,35,52,28,57,27,59},new int[]{-23,81,-21,82,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[81] = new State(-45);
    states[82] = new State(new int[]{8,85,10,86,9,87,11,88,22,89,12,90},new int[]{-27,83});
    states[83] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,84,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[84] = new State(-46);
    states[85] = new State(-60);
    states[86] = new State(-61);
    states[87] = new State(-62);
    states[88] = new State(-63);
    states[89] = new State(-64);
    states[90] = new State(-65);
    states[91] = new State(-23);
    states[92] = new State(new int[]{45,99,17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,93,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[93] = new State(new int[]{44,94});
    states[94] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,95,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[95] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,96,-4,19});
    states[96] = new State(new int[]{51,97});
    states[97] = new State(new int[]{43,98});
    states[98] = new State(-31);
    states[99] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,100,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[100] = new State(new int[]{47,101});
    states[101] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,102,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[102] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,103,-4,19});
    states[103] = new State(new int[]{51,104});
    states[104] = new State(new int[]{43,105});
    states[105] = new State(-32);
    states[106] = new State(new int[]{33,78,34,79,36,80,17,27,24,30,26,39,30,45,35,52,28,57,27,59},new int[]{-23,107,-21,82,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[107] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,108,-4,19});
    states[108] = new State(new int[]{51,109});
    states[109] = new State(new int[]{48,110});
    states[110] = new State(-33);
    states[111] = new State(-24);
    states[112] = new State(new int[]{21,114,17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,113,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[113] = new State(-34);
    states[114] = new State(-35);
    states[115] = new State(-25);
    states[116] = new State(-36);
    states[117] = new State(-26);
    states[118] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,119,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[119] = new State(-37);
    states[120] = new State(new int[]{17,27,24,30,26,39,30,45,35,52,33,54,34,55,28,57,27,59},new int[]{-21,121,-24,26,-25,32,-22,33,-26,49,-13,50,-29,51,-30,53,-31,56,-32,58});
    states[121] = new State(-38);
    states[122] = new State(-27);
    states[123] = new State(new int[]{13,125,18,-12},new int[]{-12,124});
    states[124] = new State(-9);
    states[125] = new State(new int[]{26,128},new int[]{-11,126});
    states[126] = new State(new int[]{13,125,18,-12},new int[]{-12,127});
    states[127] = new State(-11);
    states[128] = new State(new int[]{22,129,32,132,13,-16,18,-16},new int[]{-9,134});
    states[129] = new State(new int[]{35,52,33,54,34,55,28,57,27,59},new int[]{-13,130,-29,51,-30,53,-31,56,-32,58});
    states[130] = new State(new int[]{32,132,13,-16,18,-16},new int[]{-9,131});
    states[131] = new State(-13);
    states[132] = new State(new int[]{31,133});
    states[133] = new State(-15);
    states[134] = new State(-14);
    states[135] = new State(-6);
    states[136] = new State(new int[]{26,137});
    states[137] = new State(new int[]{17,138});
    states[138] = new State(new int[]{26,128,18,-10},new int[]{-8,139,-11,123});
    states[139] = new State(new int[]{18,140});
    states[140] = new State(new int[]{32,132,21,-16,51,-16,26,-16,41,-16,43,-16,48,-16,39,-16,38,-16,40,-16,25,-16,17,-16,24,-16,30,-16,35,-16,33,-16,34,-16,28,-16,27,-16},new int[]{-9,141});
    states[141] = new State(new int[]{21,8,26,-18,41,-18,43,-18,48,-18,39,-18,38,-18,40,-18,25,-18,17,-18,24,-18,30,-18,35,-18,33,-18,34,-18,28,-18,27,-18,51,-18},new int[]{-10,142,-4,19});
    states[142] = new State(new int[]{51,143});
    states[143] = new State(new int[]{50,144});
    states[144] = new State(-7);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-2, new int[]{-1,3});
    rules[2] = new Rule(-1, new int[]{-3,3});
    rules[3] = new Rule(-3, new int[]{-4,-5,-3});
    rules[4] = new Rule(-3, new int[]{-4});
    rules[5] = new Rule(-5, new int[]{-6});
    rules[6] = new Rule(-5, new int[]{-7});
    rules[7] = new Rule(-7, new int[]{50,26,17,-8,18,-9,-10,51,50});
    rules[8] = new Rule(-6, new int[]{49,26,17,-8,18,-10,51,49});
    rules[9] = new Rule(-8, new int[]{-11,-12});
    rules[10] = new Rule(-8, new int[]{});
    rules[11] = new Rule(-12, new int[]{13,-11,-12});
    rules[12] = new Rule(-12, new int[]{});
    rules[13] = new Rule(-11, new int[]{26,22,-13,-9});
    rules[14] = new Rule(-11, new int[]{26,-9});
    rules[15] = new Rule(-9, new int[]{32,31});
    rules[16] = new Rule(-9, new int[]{});
    rules[17] = new Rule(-4, new int[]{21,-4});
    rules[18] = new Rule(-4, new int[]{});
    rules[19] = new Rule(-10, new int[]{-4,-14,-10});
    rules[20] = new Rule(-10, new int[]{-4});
    rules[21] = new Rule(-14, new int[]{-15});
    rules[22] = new Rule(-14, new int[]{-16});
    rules[23] = new Rule(-14, new int[]{-17});
    rules[24] = new Rule(-14, new int[]{-18});
    rules[25] = new Rule(-14, new int[]{-19});
    rules[26] = new Rule(-14, new int[]{-20});
    rules[27] = new Rule(-14, new int[]{-21});
    rules[28] = new Rule(-15, new int[]{-22,22,-21});
    rules[29] = new Rule(-16, new int[]{41,-23,-10,51,41});
    rules[30] = new Rule(-16, new int[]{41,-23,-10,42,-10,51,41});
    rules[31] = new Rule(-17, new int[]{43,-21,44,-21,-10,51,43});
    rules[32] = new Rule(-17, new int[]{43,45,-21,47,-21,-10,51,43});
    rules[33] = new Rule(-17, new int[]{48,-23,-10,51,48});
    rules[34] = new Rule(-18, new int[]{39,-21});
    rules[35] = new Rule(-18, new int[]{39,21});
    rules[36] = new Rule(-19, new int[]{38});
    rules[37] = new Rule(-20, new int[]{40,-21});
    rules[38] = new Rule(-20, new int[]{25,-21});
    rules[39] = new Rule(-21, new int[]{-24});
    rules[40] = new Rule(-21, new int[]{-25});
    rules[41] = new Rule(-21, new int[]{-26});
    rules[42] = new Rule(-21, new int[]{-13});
    rules[43] = new Rule(-23, new int[]{33});
    rules[44] = new Rule(-23, new int[]{34});
    rules[45] = new Rule(-23, new int[]{36,-23});
    rules[46] = new Rule(-23, new int[]{-21,-27,-21});
    rules[47] = new Rule(-25, new int[]{-22,17,-28,18});
    rules[48] = new Rule(-25, new int[]{30,17,-28,18});
    rules[49] = new Rule(-28, new int[]{-21,-28});
    rules[50] = new Rule(-28, new int[]{});
    rules[51] = new Rule(-24, new int[]{17,-21,18});
    rules[52] = new Rule(-24, new int[]{24,-21});
    rules[53] = new Rule(-26, new int[]{-22,23,-21});
    rules[54] = new Rule(-26, new int[]{-22,24,-21});
    rules[55] = new Rule(-26, new int[]{-22,7,-21});
    rules[56] = new Rule(-26, new int[]{-22,14,-21});
    rules[57] = new Rule(-22, new int[]{26});
    rules[58] = new Rule(-22, new int[]{26,5,-22});
    rules[59] = new Rule(-22, new int[]{26,15,-21,16});
    rules[60] = new Rule(-27, new int[]{8});
    rules[61] = new Rule(-27, new int[]{10});
    rules[62] = new Rule(-27, new int[]{9});
    rules[63] = new Rule(-27, new int[]{11});
    rules[64] = new Rule(-27, new int[]{22});
    rules[65] = new Rule(-27, new int[]{12});
    rules[66] = new Rule(-13, new int[]{-29});
    rules[67] = new Rule(-13, new int[]{-30});
    rules[68] = new Rule(-13, new int[]{-31});
    rules[69] = new Rule(-13, new int[]{-32});
    rules[70] = new Rule(-29, new int[]{35});
    rules[71] = new Rule(-30, new int[]{33});
    rules[72] = new Rule(-30, new int[]{34});
    rules[73] = new Rule(-31, new int[]{28});
    rules[74] = new Rule(-32, new int[]{27});
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

#line 194 "SpecFiles\BrightScript.y"
 #line default
}
}
