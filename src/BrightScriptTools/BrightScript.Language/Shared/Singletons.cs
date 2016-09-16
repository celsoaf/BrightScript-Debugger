using System;
using System.ComponentModel.Composition;
using BrightScript.Language.Formatting;
using BrightScript.Language.Navigation;
using BrightScript.Language.Text;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Operations;

namespace BrightScript.Language.Shared
{
    [Export(typeof(ISingletons))]
    internal class Singletons : ISingletons
    {
        [Import] private IEditorOperationsFactoryService editorOperationsFactory = null;

        [Import] private IVsEditorAdaptersFactoryService editorAdaptersFactory = null;

        [Import] private SVsServiceProvider serviceProvider = null;

        [Import]
        private Lazy<GlobalEditorOptions> globalEditorOptions = null;


        private SourceTextCache sourceTextCache;
        private FeatureContainer featureContainer;
        private Formatting.UserSettings userSettings;
        private IDocumentOperations documentOperations;

        public IVsEditorAdaptersFactoryService EditorAdaptersFactory
        {
            get { return this.editorAdaptersFactory; }
        }

        public FeatureContainer FeatureContainer
        {
            get
            {
                if (this.featureContainer == null)
                {
                    this.featureContainer = new FeatureContainer(this);
                }

                return this.featureContainer;
            }
        }

        public Formatting.UserSettings FormattingUserSettings
        {
            get
            {
                if (this.userSettings == null)
                {
                    var shell = this.serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
                    Assumes.Present(shell);
                    Guid guid = Guids.Package;
                    IVsPackage package;
                    //ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref guid, out package));
                    shell.LoadPackage(ref guid, out package);
                    if (package != null)
                    {
                        LanguageServicePackage bsPackage = (LanguageServicePackage) package;
                        this.userSettings = bsPackage.FormattingUserSettings;
                    }
                    this.globalEditorOptions.Value.Initialize();
                }

                return this.userSettings;
            }
        }

        public IDocumentOperations DocumentOperations
        {
            get
            {
                if (this.documentOperations == null)
                {
                    this.documentOperations = new DocumentOperations(this);
                }

                return this.documentOperations;
            }

            internal set
            {
                // Used in unit-tests
                this.documentOperations = value;
            }
        }

        public IEditorOperationsFactoryService EditorOperationsFactory
        {
            get { return this.editorOperationsFactory; }
        }

        public IServiceProvider ServiceProvider
        {
            get { return this.serviceProvider; }
        }

        public SourceTextCache SourceTextCache
        {
            get
            {
                return this.sourceTextCache != null ? this.sourceTextCache : (this.sourceTextCache = new SourceTextCache());
            }
        }
    }
}