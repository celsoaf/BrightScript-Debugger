//***************************************************************************
// 
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using Microsoft.VisualStudio.Text.Tagging;

namespace BrightScript.Language.Classification
{
    public class BrightScriptTokenTag : ITag 
    {
        public BrightScriptTokenTypes type { get; private set; }

        public BrightScriptTokenTag(BrightScriptTokenTypes type)
        {
            this.type = type;
        }
    }
}
