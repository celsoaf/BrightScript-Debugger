using System.Collections.Generic;
using System.Linq;
using BrightScript.Debugger.Engine;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    internal class AD7DisassemblyStream : IDebugDisassemblyStream2
    {
        private AD7Engine _engine;
        private ulong _addr;
        private enum_DISASSEMBLY_STREAM_SCOPE _scope;

        internal AD7DisassemblyStream(AD7Engine engine, enum_DISASSEMBLY_STREAM_SCOPE scope, IDebugCodeContext2 pCodeContext)
        {
            _engine = engine;
            _scope = scope;
            AD7MemoryAddress addr = pCodeContext as AD7MemoryAddress;
            _addr = addr.Address;
        }

        #region IDebugDisassemblyStream2 Members

        public int GetCodeContext(ulong uCodeLocationId, out IDebugCodeContext2 ppCodeContext)
        {
            ppCodeContext = new AD7MemoryAddress(_engine, (uint)uCodeLocationId, null);
            return VSConstants.S_OK;
        }

        public int GetCodeLocationId(IDebugCodeContext2 pCodeContext, out ulong puCodeLocationId)
        {
            AD7MemoryAddress addr = pCodeContext as AD7MemoryAddress;
            puCodeLocationId = addr.Address;
            return VSConstants.S_OK;
        }

        public int GetCurrentLocation(out ulong puCodeLocationId)
        {
            puCodeLocationId = _addr;
            return VSConstants.S_OK;
        }

        public int GetDocument(string bstrDocumentUrl, out IDebugDocument2 ppDocument)
        {
            // Mixed mode not yet
            ppDocument = null;
            return VSConstants.S_FALSE;
        }

        public int GetScope(enum_DISASSEMBLY_STREAM_SCOPE[] pdwScope)
        {
            pdwScope[0] = _scope;
            return VSConstants.S_OK;
        }

        public int GetSize(out ulong pnSize)
        {
            pnSize = 0xFFFFFFFF;
            return VSConstants.S_OK;
        }

        public int Read(uint dwInstructions, enum_DISASSEMBLY_STREAM_FIELDS dwFields, out uint pdwInstructionsRead, DisassemblyData[] prgDisassembly)
        {
            uint iOp = 0;

            //IEnumerable<DisasmInstruction> instructions = null;
            //_engine.DebuggedProcess.WorkerThread.RunOperation(async () =>
            //{
            //    instructions = await _engine.DebuggedProcess.Disassembly.FetchInstructions(_addr, (int)dwInstructions);
            //});

            //if (instructions != null)
            //{
            //    foreach (DisasmInstruction instruction in instructions)
            //    {
            //        if (iOp >= dwInstructions)
            //        {
            //            break;
            //        }
            //        _addr = instruction.Addr;

            //        if ((dwFields & enum_DISASSEMBLY_STREAM_FIELDS.DSF_ADDRESS) != 0)
            //        {
            //            prgDisassembly[iOp].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_ADDRESS;
            //            prgDisassembly[iOp].bstrAddress = instruction.AddressString;
            //        }

            //        if ((dwFields & enum_DISASSEMBLY_STREAM_FIELDS.DSF_CODELOCATIONID) != 0)
            //        {
            //            prgDisassembly[iOp].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_CODELOCATIONID;
            //            prgDisassembly[iOp].uCodeLocationId = instruction.Addr;
            //        }

            //        if ((dwFields & enum_DISASSEMBLY_STREAM_FIELDS.DSF_SYMBOL) != 0)
            //        {
            //            if (instruction.Offset == 0)
            //            {
            //                prgDisassembly[iOp].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_SYMBOL;
            //                prgDisassembly[iOp].bstrSymbol = instruction.Symbol ?? string.Empty;
            //            }
            //        }

            //        if ((dwFields & enum_DISASSEMBLY_STREAM_FIELDS.DSF_OPCODE) != 0)
            //        {
            //            prgDisassembly[iOp].dwFields |= enum_DISASSEMBLY_STREAM_FIELDS.DSF_OPCODE;
            //            prgDisassembly[iOp].bstrOpcode = instruction.Opcode;
            //        }

            //        iOp++;
            //    };
            //}

            pdwInstructionsRead = iOp;

            return pdwInstructionsRead != 0 ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public int Seek(enum_SEEK_START dwSeekStart, IDebugCodeContext2 pCodeContext, ulong uCodeLocationId, long iInstructions)
        {
            if (dwSeekStart == enum_SEEK_START.SEEK_START_CODECONTEXT)
            {
                AD7MemoryAddress addr = pCodeContext as AD7MemoryAddress;
                _addr = addr.Address;
            }
            else if (dwSeekStart == enum_SEEK_START.SEEK_START_CODELOCID)
            {
                _addr = (uint)uCodeLocationId;
            }

            //if (iInstructions != 0)
            //{
            //    IEnumerable<DisasmInstruction> instructions = null;
            //    _engine.DebuggedProcess.WorkerThread.RunOperation(async () =>
            //    {
            //        instructions = await _engine.DebuggedProcess.Disassembly.FetchInstructions(_addr, (int)iInstructions);
            //    });
            //    if (instructions == null)
            //    {
            //        return VSConstants.E_FAIL;
            //    }
            //    _addr = instructions.ElementAt(0).Addr;
            //}
            return VSConstants.S_OK;
        }

        #endregion
    }
}