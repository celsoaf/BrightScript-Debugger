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
    /// Defines the editor format for the ookExclamation classification type. Text is colored BlueViolet
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ook!")]
    [Name("ook!")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class OokE : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "exclamation" classification type
        /// </summary>
        public OokE()
        {
            DisplayName = "ook!"; //human readable version of the name
            ForegroundColor = Colors.BlueViolet;
        }
    }

    /// <summary>
    /// Defines the editor format for the ookQuestion classification type. Text is colored Green
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ook?")]
    [Name("ook?")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class OokQ : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "question" classification type
        /// </summary>
        public OokQ()
        {
            DisplayName = "ook?"; //human readable version of the name
            ForegroundColor = Colors.Green;
        }
    }

    /// <summary>
    /// Defines the editor format for the ookPeriod classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "ook.")]
    [Name("ook.")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class OokP : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public OokP()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Orange;
        }
    }



    /// <summary>
    /// Defines the editor format for the bsCmnt classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Cmnt")]
    [Name("Cmnt")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsCmnt : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsCmnt()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.DarkGray;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsKeyword classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Keyword")]
    [Name("Keyword")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsKeyword : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsKeyword()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Purple;
        }
    }

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
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Black;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsStr classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Str")]
    [Name("Str")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsStr : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsStr()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Orange;
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
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Chartreuse;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsNumber classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Number")]
    [Name("Number")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsNumber : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsNumber()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Black;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsIdent classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Ident")]
    [Name("Ident")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsIdent : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsIdent()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Chocolate;
        }
    }

    /// <summary>
    /// Defines the editor format for the bsOpr classification type. Text is colored Orange
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Opr")]
    [Name("Opr")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class BsOpr : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "period" classification type
        /// </summary>
        public BsOpr()
        {
            DisplayName = "ook."; //human readable version of the name
            ForegroundColor = Colors.Black;
        }
    }
    #endregion //Format definition
}
