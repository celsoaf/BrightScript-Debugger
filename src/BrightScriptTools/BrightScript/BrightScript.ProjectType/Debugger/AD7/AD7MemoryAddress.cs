﻿using System;
using BrightScript.Debugger.Engine;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    // And implementation of IDebugCodeContext2 and IDebugMemoryContext2. 
    // IDebugMemoryContext2 represents a position in the address space of the machine running the program being debugged.
    // IDebugCodeContext2 represents the starting position of a code instruction. 
    // For most run-time architectures today, a code context can be thought of as an address in a program's execution stream.
    internal class AD7MemoryAddress : IDebugCodeContext2
    {
        private readonly AD7Engine _engine;
        private readonly ulong _address;
        /*OPTIONAL*/
        private string _functionName;
        private IDebugDocumentContext2 _documentContext;

        public AD7MemoryAddress(AD7Engine engine, ulong address, /*OPTIONAL*/ string functionName)
        {
            _engine = engine;
            _address = address;
            _functionName = functionName;
        }

        internal ulong Address { get { return _address; } }
        internal AD7Engine Engine { get { return _engine; } }

        public void SetDocumentContext(IDebugDocumentContext2 docContext)
        {
            _documentContext = docContext;
        }

        #region IDebugMemoryContext2 Members

        // Adds a specified value to the current context's address to create a new context.
        public int Add(ulong dwCount, out IDebugMemoryContext2 newAddress)
        {
            newAddress = new AD7MemoryAddress(_engine, (uint)dwCount + _address, null);
            return VSConstants.S_OK;
        }

        // Compares the memory context to each context in the given array in the manner indicated by compare flags, 
        // returning an index of the first context that matches.
        public int Compare(enum_CONTEXT_COMPARE contextCompare, IDebugMemoryContext2[] compareToItems, uint compareToLength, out uint foundIndex)
        {
            foundIndex = uint.MaxValue;

            try
            {
                for (uint c = 0; c < compareToLength; c++)
                {
                    AD7MemoryAddress compareTo = compareToItems[c] as AD7MemoryAddress;
                    if (compareTo == null)
                    {
                        continue;
                    }

                    if (!AD7Engine.ReferenceEquals(_engine, compareTo._engine))
                    {
                        continue;
                    }

                    bool result;

                    switch (contextCompare)
                    {
                        case enum_CONTEXT_COMPARE.CONTEXT_EQUAL:
                            result = (_address == compareTo._address);
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN:
                            result = (_address < compareTo._address);
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN:
                            result = (_address > compareTo._address);
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_LESS_THAN_OR_EQUAL:
                            result = (_address <= compareTo._address);
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_GREATER_THAN_OR_EQUAL:
                            result = (_address >= compareTo._address);
                            break;

                        // The debug engine doesn't understand scopes
                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_SCOPE:
                            result = (_address == compareTo._address);
                            break;

                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_FUNCTION:
                            if (_address == compareTo._address)
                            {
                                result = true;
                                break;
                            }
                            string funcThis = Engine.GetAddressDescription(_address);
                            if (string.IsNullOrEmpty(funcThis))
                            {
                                result = false;
                                break;
                            }
                            string funcCompareTo = Engine.GetAddressDescription(compareTo._address);
                            result = (funcThis == funcCompareTo);
                            break;

                        //case enum_CONTEXT_COMPARE.CONTEXT_SAME_MODULE:
                        //    result = (_address == compareTo._address);
                        //    if (result == false)
                        //    {
                        //        DebuggedModule module = _engine.DebuggedProcess.ResolveAddress(_address);

                        //        if (module != null)
                        //        {
                        //            result = module.AddressInModule(compareTo._address);
                        //        }
                        //    }
                        //    break;

                        case enum_CONTEXT_COMPARE.CONTEXT_SAME_PROCESS:
                            result = true;
                            break;

                        default:
                            // A new comparison was invented that we don't support
                            return VSConstants.E_NOTIMPL;
                    }

                    if (result)
                    {
                        foundIndex = c;
                        return VSConstants.S_OK;
                    }
                }

                return VSConstants.S_FALSE;
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

        // Gets information that describes this context.
        public int GetInfo(enum_CONTEXT_INFO_FIELDS dwFields, CONTEXT_INFO[] pinfo)
        {
            try
            {
                pinfo[0].dwFields = 0;

                if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS) != 0)
                {
                    //pinfo[0].bstrAddress = EngineUtils.AsAddr(_address, _engine.DebuggedProcess.Is64BitArch);
                    pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESS;
                }

                // Fields not supported by the sample
                if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSOFFSET) != 0) { }
                if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSABSOLUTE) != 0)
                {
                    //pinfo[0].bstrAddressAbsolute = EngineUtils.AsAddr(_address, _engine.DebuggedProcess.Is64BitArch);
                    pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_ADDRESSABSOLUTE;
                }
                //if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_MODULEURL) != 0)
                //{
                //    DebuggedModule module = _engine.DebuggedProcess.ResolveAddress(_address);
                //    if (module != null)
                //    {
                //        pinfo[0].bstrModuleUrl = module.Name;
                //        pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_MODULEURL;
                //    }
                //}
                if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTION) != 0)
                {
                    if (_functionName == null)
                    {
                        _functionName = Engine.GetAddressDescription(_address);
                    }

                    if (!(string.IsNullOrEmpty(_functionName) || _functionName[0] == '0' /*address*/))
                    {
                        pinfo[0].bstrFunction = _functionName;
                        pinfo[0].dwFields |= enum_CONTEXT_INFO_FIELDS.CIF_FUNCTION;
                    }
                }
                if ((dwFields & enum_CONTEXT_INFO_FIELDS.CIF_FUNCTIONOFFSET) != 0) { }

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

        // Gets the user-displayable name for this context
        // This is not supported by the sample engine.
        public int GetName(out string pbstrName)
        {
            throw new NotImplementedException();
        }

        // Subtracts a specified value from the current context's address to create a new context.
        public int Subtract(ulong dwCount, out IDebugMemoryContext2 ppMemCxt)
        {
            ppMemCxt = new AD7MemoryAddress(_engine, _address - (uint)dwCount, null);
            return VSConstants.S_OK;
        }

        #endregion

        #region IDebugCodeContext2 Members

        // Gets the document context for this code-context
        public int GetDocumentContext(out IDebugDocumentContext2 ppSrcCxt)
        {
            ppSrcCxt = _documentContext;
            return VSConstants.S_OK;
        }

        // Gets the language information for this code context.
        public int GetLanguageInfo(ref string pbstrLanguage, ref Guid pguidLanguage)
        {
            if (_documentContext != null)
            {
                return _documentContext.GetLanguageInfo(ref pbstrLanguage, ref pguidLanguage);
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        #endregion
    }
}