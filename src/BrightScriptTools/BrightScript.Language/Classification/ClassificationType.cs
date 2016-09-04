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
        /// Defines the "ookExclamation" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook!")]
        internal static ClassificationTypeDefinition ookExclamation = null;

        /// <summary>
        /// Defines the "ookQuestion" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook?")]
        internal static ClassificationTypeDefinition ookQuestion = null;

        /// <summary>
        /// Defines the "ookPeriod" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook.")]
        internal static ClassificationTypeDefinition ookPeriod = null;


        /// <summary>
        /// Defines the "bsOpr" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Opr")]
        internal static ClassificationTypeDefinition bsOpr = null;

        /// <summary>
        /// Defines the "bsFuncs" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Funcs")]
        internal static ClassificationTypeDefinition bsFuncs = null;

        /// <summary>
        /// Defines the "bsIdent" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Ident")]
        internal static ClassificationTypeDefinition bsIdent = null;

        /// <summary>
        /// Defines the "bsKeyword" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Keyword")]
        internal static ClassificationTypeDefinition bsKeyword = null;

        /// <summary>
        /// Defines the "bsNumber" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Number")]
        internal static ClassificationTypeDefinition bsNumber = null;

        /// <summary>
        /// Defines the "bsStr" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Str")]
        internal static ClassificationTypeDefinition bsStr = null;

        /// <summary>
        /// Defines the "bsTyps" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Typs")]
        internal static ClassificationTypeDefinition bsTyps = null;

        /// <summary>
        /// Defines the "bsCmnt" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("Cmnt")]
        internal static ClassificationTypeDefinition bsCmnt = null;

        #endregion
    }
}
