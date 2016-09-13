using System;
using BrightScript.Language.Formatting;
using BrightScript.Language.Navigation;
using BrightScript.Language.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace BrightScript.Language.Shared
{
    internal interface ISingletons
    {
        IDocumentOperations DocumentOperations { get; }

        IEditorOperationsFactoryService EditorOperationsFactory { get; }

        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; }

        FeatureContainer FeatureContainer { get; }

        UserSettings FormattingUserSettings { get; }

        IServiceProvider ServiceProvider { get; }

        SourceTextCache SourceTextCache { get; }
    }
}