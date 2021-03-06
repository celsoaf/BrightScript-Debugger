﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using BrightScript.Debugger.AD7.Defenitions;
using BrightScript.Debugger.Engine;
using BrightScript.Debugger.Exceptions;
using BrightScript.Debugger.Models;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    // Represents a logical stack frame on the thread stack. 
    // Also implements the IDebugExpressionContext interface, which allows expression evaluation and watch windows.
    internal class AD7StackFrame : IDebugStackFrame2, IDebugExpressionContext2
    {
        public AD7Engine Engine { get; private set; }
        public AD7Thread Thread { get; private set; }
        public ThreadContext ThreadContext { get; private set; }

        private string _functionName;
        private MITextPosition _textPosition;
        private bool _hasGottenLocalsAndParams = false;
        private uint _radix;
        private AD7MemoryAddress _codeCxt;
        private AD7DocumentContext _documentCxt;

        // An array of this frame's parameters
        private readonly List<VariableInformation> _parameters = new List<VariableInformation>();

        // An array of this frame's locals
        private readonly List<VariableInformation> _locals = new List<VariableInformation>();


        public AD7StackFrame(AD7Engine engine, AD7Thread thread, ThreadContext threadContext)
        {
            Debug.Assert(threadContext != null, "ThreadContext is null");

            Engine = engine;
            Thread = thread;
            ThreadContext = threadContext;

            _textPosition = threadContext.TextPosition;
            _functionName = threadContext.Function;

            if (_textPosition != null)
            {
                _documentCxt = new AD7DocumentContext(_textPosition, _codeCxt);

                if (_codeCxt != null)
                {
                    _codeCxt.SetDocumentContext(_documentCxt);
                }
            }
        }

        #region Non-interface methods

        public void SetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, out FRAMEINFO frameInfo)
        {
            List<SimpleVariableInformation> parameters = null;
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS) != 0 && !this.Engine.DebuggedProcess.CommandFactory.SupportsFrameFormatting)
            {
                Engine.DebuggedProcess.WorkerThread.RunOperation(async () =>
                {
                    parameters = await Engine.DebuggedProcess.GetParameterInfoOnly(Thread, ThreadContext);
                });
            }
            SetFrameInfo(dwFieldSpec, out frameInfo, parameters);
        }

        // Construct a FRAMEINFO for this stack frame with the requested information.
        public void SetFrameInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, out FRAMEINFO frameInfo, List<SimpleVariableInformation> parameters)
        {
            frameInfo = new FRAMEINFO();

            //DebuggedModule module = ThreadContext.FindModule(Engine.DebuggedProcess);

            // The debugger is asking for the formatted name of the function which is displayed in the callstack window.
            // There are several optional parts to this name including the module, argument types and values, and line numbers.
            // The optional information is requested by setting flags in the dwFieldSpec parameter.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME) != 0)
            {
                // If there is source information, construct a string that contains the module name, function name, and optionally argument names and values.
                if (_textPosition != null)
                {
                    frameInfo.m_bstrFuncName = "";

                    //if (module != null && (dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_MODULE) != 0)
                    //{
                    //    frameInfo.m_bstrFuncName = System.IO.Path.GetFileName(module.Name) + "!";
                    //}

                    frameInfo.m_bstrFuncName += _functionName;
                    
                    if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS) != 0 && !Engine.DebuggedProcess.CommandFactory.SupportsFrameFormatting)
                    {
                        //frameInfo.m_bstrFuncName += "(";
                        if (parameters != null && parameters.Count > 0)
                        {
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_TYPES) != 0)
                                {
                                    frameInfo.m_bstrFuncName += parameters[i].TypeName + " ";
                                }

                                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_NAMES) != 0)
                                {
                                    frameInfo.m_bstrFuncName += parameters[i].Name;
                                }

                                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_NAMES) != 0 &&
                                    (dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_VALUES) != 0)
                                {
                                    frameInfo.m_bstrFuncName += "=";
                                }

                                if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_ARGS_VALUES) != 0)
                                {
                                    frameInfo.m_bstrFuncName += parameters[i].Value;
                                }

                                if (i < parameters.Count - 1)
                                {
                                    frameInfo.m_bstrFuncName += ", ";
                                }
                            }
                        }
                        //frameInfo.m_bstrFuncName += ")";
                    }

                    if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FUNCNAME_LINES) != 0)
                    {
                        frameInfo.m_bstrFuncName += string.Format(CultureInfo.CurrentCulture, " - Line {0}", _textPosition.BeginPosition.dwLine + 1);
                    }
                }
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FUNCNAME;
            }

            // The debugger is requesting the range of memory addresses for this frame.
            // For the sample engine, this is the contents of the frame pointer.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_STACKRANGE) != 0)
            {
                frameInfo.m_addrMin = ThreadContext.sp;
                frameInfo.m_addrMax = ThreadContext.sp;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STACKRANGE;
            }

            // The debugger is requesting the IDebugStackFrame2 value for this frame info.
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FRAME) != 0)
            {
                frameInfo.m_pFrame = this;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FRAME;
            }

            // Does this stack frame of symbols loaded?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO) != 0)
            {
                frameInfo.m_fHasDebugInfo = _textPosition != null ? 1 : 0;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_DEBUGINFO;
            }

            // Is this frame stale?
            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_STALECODE) != 0)
            {
                frameInfo.m_fStaleCode = 0;
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_STALECODE;
            }

            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_FLAGS) != 0)
            {
                if (_codeCxt == null && _documentCxt == null)
                {
                    frameInfo.m_dwFlags |= (uint)enum_FRAMEINFO_FLAGS_VALUES.FIFV_ANNOTATEDFRAME;
                }
                frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_FLAGS;
            }

            if ((dwFieldSpec & enum_FRAMEINFO_FLAGS.FIF_LANGUAGE) != 0)
            {
                Guid unused = Guid.Empty;
                if (GetLanguageInfo(ref frameInfo.m_bstrLanguage, ref unused) == 0)
                {
                    frameInfo.m_dwValidFields |= enum_FRAMEINFO_FLAGS.FIF_LANGUAGE;
                }
            }
        }

        private void EnsureLocalsAndParameters()
        {
            uint radix = Engine.CurrentRadix();
            if (_textPosition != null && (!_hasGottenLocalsAndParams || radix != _radix))
            {
                _radix = radix;
                List<VariableInformation> localsAndParameters = null;
                Engine.DebuggedProcess.WorkerThread.RunOperation(async () =>
                {
                    localsAndParameters = await Engine.DebuggedProcess.GetLocalsAndParameters(Thread, ThreadContext);
                });

                _parameters.Clear();
                _locals.Clear();
                foreach (VariableInformation vi in localsAndParameters)
                {
                    if (vi.IsParameter)
                    {
                        _parameters.Add(vi);
                    }
                    else
                    {
                        _locals.Add(vi);
                    }
                }

                _hasGottenLocalsAndParams = true;
            }
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the combined locals and parameters.
        private void CreateLocalsPlusArgsProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = 0;

            int localsLength = 0;

            if (_locals != null)
            {
                localsLength = _locals.Count;
                elementsReturned += (uint)localsLength;
            }

            if (_parameters != null)
            {
                elementsReturned += (uint)_parameters.Count;
            }
            DEBUG_PROPERTY_INFO[] propInfo = new DEBUG_PROPERTY_INFO[elementsReturned];

            if (_locals != null)
            {
                for (int i = 0; i < _locals.Count; i++)
                {
                    AD7Property property = new AD7Property(Engine, _locals[i]);
                    propInfo[i] = property.ConstructDebugPropertyInfo(dwFields);
                }
            }

            if (_parameters != null)
            {
                for (int i = 0; i < _parameters.Count; i++)
                {
                    AD7Property property = new AD7Property(Engine, _parameters[i]);
                    propInfo[localsLength + i] = property.ConstructDebugPropertyInfo(dwFields);
                }
            }

            enumObject = new AD7PropertyInfoEnum(propInfo);
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the locals collection only.
        private void CreateLocalProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = (uint)_locals.Count;
            DEBUG_PROPERTY_INFO[] propInfo = new DEBUG_PROPERTY_INFO[_locals.Count];

            for (int i = 0; i < propInfo.Length; i++)
            {
                AD7Property property = new AD7Property(Engine, _locals[i]);
                propInfo[i] = property.ConstructDebugPropertyInfo(dwFields);
            }

            enumObject = new AD7PropertyInfoEnum(propInfo);
        }

        // Construct an instance of IEnumDebugPropertyInfo2 for the parameters collection only.
        private void CreateParameterProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            elementsReturned = (uint)_parameters.Count;
            DEBUG_PROPERTY_INFO[] propInfo = new DEBUG_PROPERTY_INFO[_parameters.Count];

            for (int i = 0; i < propInfo.Length; i++)
            {
                AD7Property property = new AD7Property(Engine, _parameters[i]);
                propInfo[i] = property.ConstructDebugPropertyInfo(dwFields);
            }

            enumObject = new AD7PropertyInfoEnum(propInfo);
        }

        public string EvaluateExpression(string expr)
        {
            string val = null;
            Engine.DebuggedProcess.WorkerThread.RunOperation(async () =>
            {
                val = await Engine.DebuggedProcess.CommandFactory.DataEvaluateExpression(expr, Thread.Id, ThreadContext.Level);
            });
            return val;
        }

        #endregion

        #region IDebugStackFrame2 Members

        // Creates an enumerator for properties associated with the stack frame, such as local variables.
        // The sample engine only supports returning locals and parameters. Other possible values include
        // class fields (this pointer), registers, exceptions...
        int IDebugStackFrame2.EnumProperties(enum_DEBUGPROP_INFO_FLAGS dwFields, uint nRadix, ref Guid guidFilter, uint dwTimeout, out uint elementsReturned, out IEnumDebugPropertyInfo2 enumObject)
        {
            int hr;

            elementsReturned = 0;
            enumObject = null;

            try
            {
                if (guidFilter == AD7Guids.guidFilterAllLocals ||
                    guidFilter == AD7Guids.guidFilterAllLocalsPlusArgs ||
                    guidFilter == AD7Guids.guidFilterArgs ||
                    guidFilter == AD7Guids.guidFilterLocals ||
                    guidFilter == AD7Guids.guidFilterLocalsPlusArgs)
                {
                    EnsureLocalsAndParameters();
                }

                if (guidFilter == AD7Guids.guidFilterLocalsPlusArgs ||
                        guidFilter == AD7Guids.guidFilterAllLocalsPlusArgs ||
                        guidFilter == AD7Guids.guidFilterAllLocals)
                {
                    CreateLocalsPlusArgsProperties(dwFields, out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == AD7Guids.guidFilterLocals)
                {
                    CreateLocalProperties(dwFields, out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else if (guidFilter == AD7Guids.guidFilterArgs)
                {
                    CreateParameterProperties(dwFields, out elementsReturned, out enumObject);
                    hr = VSConstants.S_OK;
                }
                else
                {
                    hr = VSConstants.E_NOTIMPL;
                }
            }
            catch (MIException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }

            return hr;
        }

        // Gets the code context for this stack frame. The code context represents the current instruction pointer in this stack frame.
        int IDebugStackFrame2.GetCodeContext(out IDebugCodeContext2 memoryAddress)
        {
            memoryAddress = _codeCxt;
            if (memoryAddress == null)
            {
                return VSConstants.E_FAIL; // annotated frame
            }

            return VSConstants.S_OK;
        }

        // Gets a description of the properties of a stack frame.
        // Calling the IDebugProperty2::EnumChildren method with appropriate filters can retrieve the local variables, method parameters, registers, and "this" 
        // pointer associated with the stack frame. The debugger calls EnumProperties to obtain these values in the sample.
        int IDebugStackFrame2.GetDebugProperty(out IDebugProperty2 property)
        {
            property = null;
            return VSConstants.E_NOTIMPL;
        }

        // Gets the document context for this stack frame. The debugger will call this when the current stack frame is changed
        // and will use it to open the correct source document for this stack frame.
        int IDebugStackFrame2.GetDocumentContext(out IDebugDocumentContext2 docContext)
        {
            if (_documentCxt == null)
            {
                docContext = null;
                return VSConstants.E_FAIL; // annotated frame
            }

            docContext = _documentCxt;
            return VSConstants.S_OK;
        }

        // Gets an evaluation context for expression evaluation within the current context of a stack frame and thread.
        // Generally, an expression evaluation context can be thought of as a scope for performing expression evaluation. 
        // Call the IDebugExpressionContext2::ParseText method to parse an expression and then call the resulting IDebugExpression2::EvaluateSync 
        // or IDebugExpression2::EvaluateAsync methods to evaluate the parsed expression.
        int IDebugStackFrame2.GetExpressionContext(out IDebugExpressionContext2 ppExprCxt)
        {
            ppExprCxt = (IDebugExpressionContext2)this;
            return VSConstants.S_OK;
        }

        // Gets a description of the stack frame.
        int IDebugStackFrame2.GetInfo(enum_FRAMEINFO_FLAGS dwFieldSpec, uint nRadix, FRAMEINFO[] pFrameInfo)
        {
            try
            {
                SetFrameInfo(dwFieldSpec, out pFrameInfo[0]);

                return VSConstants.S_OK;
            }
            catch (MIException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
        }

        // Gets the language associated with this stack frame. 
        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            if (_documentCxt != null)
            {
                return ((IDebugDocumentContext2)_documentCxt).GetLanguageInfo(ref pbstrLanguage, ref pguidLanguage);
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        // Gets the name of the stack frame.
        // The name of a stack frame is typically the name of the method being executed.
        int IDebugStackFrame2.GetName(out string name)
        {
            name = null;

            try
            {
                if (_functionName != null)
                {
                    name = _functionName;
                }

                return VSConstants.S_OK;
            }
            catch (MIException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
        }

        // Gets a machine-dependent representation of the range of physical addresses associated with a stack frame.
        int IDebugStackFrame2.GetPhysicalStackRange(out ulong addrMin, out ulong addrMax)
        {
            addrMin = ThreadContext.sp;
            addrMax = ThreadContext.sp;

            return VSConstants.S_OK;
        }

        // Gets the thread associated with a stack frame.
        int IDebugStackFrame2.GetThread(out IDebugThread2 thread)
        {
            thread = Thread;
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugExpressionContext2 Members

        // Retrieves the name of the evaluation context. 
        // The name is the description of this evaluation context. It is typically something that can be parsed by an expression evaluator 
        // that refers to this exact evaluation context. For example, in C++ the name is as follows: 
        // "{ function-name, source-file-name, module-file-name }"
        int IDebugExpressionContext2.GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        // Parses a text-based expression for evaluation.
        // The engine sample only supports locals and parameters so the only task here is to check the names in those collections.
        int IDebugExpressionContext2.ParseText(string pszCode,
                                                enum_PARSEFLAGS dwFlags,
                                                uint nRadix,
                                                out IDebugExpression2 ppExpr,
                                                out string pbstrError,
                                                out uint pichError)
        {
            pbstrError = null;
            pichError = 0;
            ppExpr = null;

            try
            {
                
                ppExpr = new AD7Expression(Engine, new VariableInformation(pszCode, ThreadContext, Engine, Thread));
                return VSConstants.S_OK;
            }
            catch (MIException e)
            {
                return e.HResult;
            }
            catch (Exception e)
            {
                return EngineUtils.UnexpectedException(e);
            }
        }

        #endregion
    }
}