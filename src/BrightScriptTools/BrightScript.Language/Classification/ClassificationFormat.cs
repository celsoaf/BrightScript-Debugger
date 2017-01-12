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
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BrightScript.Language.Classification
{
    #region Format definition
    
    /// <summary>
    /// Defines the editor format for the bsTyps classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Typs")]
    [Name("Typs")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsTyps : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsTyps()
        {
            DisplayName = "Typs"; //human readable version of the name
            ForegroundColor = Colors.DeepPink;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsFuncs classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Funcs")]
    [Name("Funcs")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsFuncs : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsFuncs()
        {
            DisplayName = "Funcs"; //human readable version of the name
            ForegroundColor = Colors.Chartreuse;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsFuncs classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Ltr")]
    [Name("Ltr")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsLiteral : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsLiteral()
        {
            DisplayName = "Ltr"; //human readable version of the name
            ForegroundColor = Colors.Salmon;
        }
    }

    #endregion //Format definition
}
