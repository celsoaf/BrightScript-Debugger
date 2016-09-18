using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BrightScript.Language.Shared;
using BrightScript.Language.Text;
using BrightScriptTools.Compiler;
using Microsoft;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace BrightScript.Language.Diagnostics
{
    internal class DiagnosticsProvider : IDiagnosticsProvider
    {
        private ParseTreeCache parseTreeCache;

        public DiagnosticsProvider(ParseTreeCache parseTreeCache)
        {
            Requires.NotNull(parseTreeCache, nameof(parseTreeCache));

            this.parseTreeCache = parseTreeCache;
        }

        public IReadOnlyList<Error> GetDiagnostics(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            SyntaxTree syntaxTree = this.parseTreeCache.Get(sourceText);

            return syntaxTree.ErrorList;
        }
    }
}