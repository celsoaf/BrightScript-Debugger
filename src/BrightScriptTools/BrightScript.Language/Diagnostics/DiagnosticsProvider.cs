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
        private ISingletons _singletons;

        private readonly ConditionalWeakTable<ITextSnapshot, IEnumerable<Error>> sources =
               new ConditionalWeakTable<ITextSnapshot, IEnumerable<Error>>();

        public DiagnosticsProvider(ISingletons singletons)
        {
            _singletons = singletons;
        }

        public IEnumerable<Error> GetDiagnostics(ITextSnapshot text)
        {
            Requires.NotNull(text, nameof(text));

            IEnumerable<Error> errors;
            if (this.sources.TryGetValue(text, out errors))
            {
                return errors;
            }

            SourceText sourceText = _singletons.SourceTextCache.Get(text);
            using (Stream stream = sourceText.GetStream())
            {
                // parse input args, and open input file
                Scanner scanner = new Scanner(stream);

                Parser parser = new Parser(scanner);
                if (!parser.Parse())
                {
                    errors = scanner.Errors;
                }
                else
                {
                    errors = new List<Error>();
                }
            }

            sources.Add(text, errors);

            return errors;
        }
    }
}