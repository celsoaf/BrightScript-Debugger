﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using BrightScript.Language.Shared;
using BrightScript.Language.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace BrightScript.Language.Intellisense
{
    internal class CompletionSource : ICompletionSource
    {
        private CompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;
        private List<Completion> m_compList;
        //private ParseTreeCache m_parseTreeCache;
        private SourceTextCache m_sourceTextCache;
        private bool m_memberlist;
        [Import]
        private ISingletons singletons;

        public CompletionSource(CompletionSourceProvider provider, ITextBuffer textBuffer)
        {
            m_textBuffer = textBuffer;
            m_sourceProvider = provider;
            //m_parseTreeCache = new ParseTreeCache();
            this.m_sourceTextCache = new SourceTextCache();
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            SourceText sourceText = this.m_sourceTextCache.Get(m_textBuffer.CurrentSnapshot);
            //SyntaxTree syntaxTree = this.m_parseTreeCache.Get(sourceText);
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(m_textBuffer.CurrentSnapshot);
            //var ch = triggerPoint.GetChar();
            //Token currentToken = null;
            int currentTokenIndex = 0;
            //for (; currentTokenIndex < syntaxTree.Tokens.Count; currentTokenIndex++)
            //{
            //    var token = syntaxTree.Tokens[currentTokenIndex];
            //    if ((token.Start <= triggerPoint.Position) && (token.Start + token.Length) >= triggerPoint.Position)
            //    {
            //        currentToken = token;
            //        break;
            //    }
            //}
            List<string> strList = new List<string>();
            //if (currentToken == null)
            //    return;
            //if (currentToken.Kind == SyntaxKind.Dot)
            //{
            //    //user inputs dot. Try search members
            //    var targetname = syntaxTree.Tokens[currentTokenIndex - 1].Text;
            //    for (int i = 0; i < syntaxTree.StatementNodeList.Count; i++)
            //    {
            //        var statement = syntaxTree.StatementNodeList[i];
            //        if (statement.StartPosition + statement.Length < triggerPoint.Position) //make sure the variable is declared before the usage
            //        {
            //            if (statement.GetType() == typeof(AssignmentStatementNode))
            //            {
            //                for (int j = 0; j < statement.Children[2].Children.Count; j++)
            //                {
            //                    if (statement.Children.Count > 2)
            //                    {
            //                        var child = statement.Children[2].Children[j];
            //                        if (child.GetType() == typeof(TableConstructorExp))
            //                        {
            //                            if (statement.Children[0].Children[j].Children[0].IsLeafNode && ((Token)(statement.Children[0].Children[j].Children[0])).Text == targetname)
            //                            {
            //                                foreach (var assignment in child.Children[1].Children)
            //                                {
            //                                    if (assignment.GetType() == typeof(AssignmentField))
            //                                        strList.Add(((Token)assignment.Children[0]).Text);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (statement.GetType() == typeof(LocalAssignmentStatementNode))
            //            {
            //                if (statement.Children.Count > 3)
            //                {
            //                    for (int j = 0; j < statement.Children[3].Children.Count; j++)
            //                    {
            //                        var child = statement.Children[3].Children[j];
            //                        if (child.GetType() == typeof(TableConstructorExp))
            //                        {
            //                            if (statement.Children[1].Children[j].IsLeafNode && ((Token)(statement.Children[1].Children[j])).Text == targetname)
            //                            {
            //                                foreach (var assignment in child.Children[1].Children)
            //                                {
            //                                    if (assignment.GetType() == typeof(AssignmentField))
            //                                        strList.Add(((Token)assignment.Children[0]).Text);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    m_memberlist = true;
            //}
            //else
            {
                //list all variables and keywords
                strList.AddRange(GetList());
                //foreach (var statement in syntaxTree.StatementNodeList)
                //{
                //    if (statement.StartPosition + statement.Length < triggerPoint.Position) //make sure the variable is declared before the usage
                //    {
                //        if (statement.GetType() == typeof(AssignmentStatementNode))
                //        {
                //            var namelist = statement.Children[0];
                //            foreach (var namevar in namelist.Children)
                //            {
                //                if (namevar.GetType() == typeof(NameVar))
                //                {
                //                    var str = ((Token)((NameVar)namevar).Children[0]).Text;
                //                    if (strList.IndexOf(str) == -1)
                //                        strList.Add(str);
                //                }
                //            }
                //        }
                //        else if (statement.GetType() == typeof(LocalAssignmentStatementNode))
                //        {
                //            foreach (var namevar in ((SeparatedList)statement.Children[1]).Children)
                //            {
                //                if (namevar.IsLeafNode)
                //                {
                //                    var str = ((Token)namevar).Text;
                //                    if (strList.IndexOf(str) == -1)
                //                        strList.Add(str);
                //                }
                //            }
                //        }
                //    }
                //}
                m_memberlist = false;
            }
            strList.Sort();
            this.m_compList = new List<Completion>();
            foreach (string str in strList)
            {
                this.m_compList.Add(new Completion(str, str, str, null, null));
            }

            completionSets.Add(new CompletionSet(
                "Tokens",    // the non-localized title of the tab 
                "Tokens",    // the display title of the tab
                this.FindTokenSpanAtPosition(session.GetTriggerPoint(this.m_textBuffer),
                    session),
                m_compList,
                null));
        }

        private IEnumerable<string> GetList()
        {
            return new List<string>()
            {
                "Abs",
                "AddAttribute",
                "AddBodyElement",
                "AddElement",
                "AddElementWithBody",
                "AddHead",
                "AddReplace",
                "AddTail",
                "Append",
                "AppendFile",
                "AppendString",
                "As",
                "Asc",
                "Atn",
                "Boolean",
                "Box",
                "Cdbl",
                "Chr",
                "Clear",
                "CopyFile",
                "Cos",
                "Count",
                "CreateDirectory",
                "CreateObject",
                "Csng",
                "Delete",
                "DeleteDirectory",
                "DeleteFile",
                "dim",
                "DoesExist",
                "Double",
                "Dynamic",
                "else",
                "elseif",
                "end",
                "endif",
                "Exit",
                "Exp",
                "Fix",
                "Float",
                "for",
                "each",
                "FormatDrive",
                "FromAsciiString",
                "FromBase64String",
                "FromHexString",
                "Function",
                "GenXML",
                "GenXMLHeader",
                "GetAttributes",
                "GetBody",
                "GetBoolean",
                "GetChildElements",
                "GetEntityEncode",
                "GetEntry",
                "GetFloat",
                "GetHead",
                "GetIndex",
                "GetInt",
                "GetInterface",
                "GetLastRunCompileError",
                "GetLastRunRuntimeError",
                "GetMessage",
                "GetName",
                "GetNamedElements",
                "GetSignedByte",
                "GetString",
                "GetSub",
                "GetTail",
                "GetText",
                "goto",
                "HasAttribute",
                "if",
                "ifArray",
                "ifAssociativeArray",
                "ifBoolean",
                "ifBrSub",
                "ifByteArray",
                "ifEnum",
                "ifFloat",
                "ifInt",
                "ifList",
                "ifMessagePort",
                "ifString",
                "ifStringOps",
                "ifXMLElement",
                "ifXMLList",
                "in",
                "Instr",
                "Int",
                "Integer",
                "Interface",
                "Invalid",
                "IsEmpty",
                "IsLittleEndianCPU",
                "IsName",
                "IsNext",
                "LCase",
                "Left",
                "Len",
                "ListDir",
                "Log",
                "Lookup",
                "MatchFiles",
                "MD5",
                "Mid",
                "Next",
                "Object",
                "Parse",
                "Peek",
                "Pop",
                "PostMessage",
                "print",
                "Push",
                "ReadAsciiFile",
                "ReadFile",
                "RebootSystem",
                "rem",
                "RemoveHead",
                "RemoveIndex",
                "RemoveTail",
                "Reset",
                "ResetIndex",
                "return",
                "Right",
                "Rnd",
                "roArray",
                "roAssociativeArray",
                "roBoolean",
                "roBrSub",
                "roByteArray",
                "roFloat",
                "roGlobal",
                "roInt",
                "roInvalid",
                "roList",
                "roMessagePort",
                "roString",
                "roXMLElement",
                "roXMLList",
                "Run",
                "SetBody",
                "SetBoolean",
                "SetEntry",
                "SetFloat",
                "SetInt",
                "SetModeCaseSensitive",
                "SetName",
                "SetResize",
                "SetString",
                "SetSub",
                "Sgn",
                "Shift",
                "Simplify",
                "Sin",
                "Sleep",
                "Sqr",
                "step",
                "stop",
                "Str",
                "Stri",
                "String",
                "Tan",
                "then",
                "to",
                "ToAsciiString",
                "ToBase64String",
                "ToHexString",
                "Tokenize",
                "Trim",
                "Type",
                "UCase",
                "Unshift",
                "UpTime",
                "Val",
                "Void",
                "Wait",
                "WaitMessage",
                "while",
                "WriteAsciiFile",
                "WriteFile"
            };
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = session.TextView.Caret.Position.BufferPosition;
            if (!m_memberlist)
            {
                currentPoint = currentPoint - 1;
            }
            var navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            var extent = navigator.GetExtentOfWord(currentPoint);
            var text = extent.Span.GetText();
            System.Diagnostics.Debug.WriteLine("t: " + text);
            if (!string.IsNullOrEmpty(text.Trim().Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty)))
            {
                return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
            }

            return currentPoint.Snapshot.CreateTrackingSpan(
                currentPoint.Position,
                0,
                SpanTrackingMode.EdgeInclusive);
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }
    }
}