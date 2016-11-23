﻿using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace BrightScript.Debugger.AD7
{
    public class AD7RegGroupProperty : IDebugProperty2
    {
        private readonly AD7Engine _engine;
        private readonly RegisterGroup _group;
        private readonly Tuple<int, string>[] _values;
        public readonly DEBUG_PROPERTY_INFO PropertyInfo;
        public AD7RegGroupProperty(AD7Engine engine, enum_DEBUGPROP_INFO_FLAGS dwFields, RegisterGroup grp, Tuple<int, string>[] values)
        {
            _engine = engine;
            _group = grp;
            _values = values;
            PropertyInfo = CreateInfo(dwFields);
        }

        private DEBUG_PROPERTY_INFO CreateInfo(enum_DEBUGPROP_INFO_FLAGS dwFields)
        {
            DEBUG_PROPERTY_INFO info = new DEBUG_PROPERTY_INFO();
            info.dwFields = 0;
            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0)
            {
                info.bstrName = _group.Name;
                info.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB) != 0)
            {
                info.dwAttrib = 0;
                info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_VALUE_READONLY;
                info.dwAttrib |= enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
                info.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;
            }

            if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP) != 0)
            {
                info.pProperty = this;
                info.dwFields |= enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP;
            }

            return info;
        }

        public int EnumChildren(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, ref Guid guidFilter, enum_DBG_ATTRIB_FLAGS dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
        {
            DEBUG_PROPERTY_INFO[] properties = new DEBUG_PROPERTY_INFO[_group.Count];
            
            ppEnum = new AD7PropertyEnum(properties);
            return VSConstants.S_OK;
        }

        public int GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
        {
            throw new NotImplementedException();
        }

        public int GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
        {
            throw new NotImplementedException();
        }

        public int GetMemoryContext(out IDebugMemoryContext2 ppMemory)
        {
            throw new NotImplementedException();
        }

        public int GetParent(out IDebugProperty2 ppParent)
        {
            throw new NotImplementedException();
        }

        public int GetPropertyInfo(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
        {
            pPropertyInfo[0] = PropertyInfo;
            rgpArgs = null;

            return VSConstants.S_OK;
        }

        public int GetReference(out IDebugReference2 ppReference)
        {
            throw new NotImplementedException();
        }

        public int GetSize(out uint pdwSize)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
        {
            throw new NotImplementedException();
        }

        public int SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
        {
            throw new NotImplementedException();
        }
    }
}