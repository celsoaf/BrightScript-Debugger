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

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Classification
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition

        /// <summary>
        /// Defines the "bsFuncs" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Funcs")]
        internal static ClassificationTypeDefinition bsFuncs = null;

        /// <summary>
        /// Defines the "bsTyps" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Typs")]
        internal static ClassificationTypeDefinition bsTyps = null;

        /// <summary>
        /// Defines the "bsTyps" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Ltr")]
        internal static ClassificationTypeDefinition bsLtr = null;

        #endregion
    }
}
