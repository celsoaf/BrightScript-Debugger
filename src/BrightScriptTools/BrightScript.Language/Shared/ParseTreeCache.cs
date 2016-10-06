using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using BrightScript.Language.Text;
using BrightScriptTools.Compiler;
using Microsoft;

namespace BrightScript.Language.Shared
{
    public class ParseTreeCache
    {
        private readonly ConditionalWeakTable<SourceText, SyntaxTree> sources =
            new ConditionalWeakTable<SourceText, SyntaxTree>();

        public SyntaxTree Get(SourceText sourceText)
        {
            Requires.NotNull(sourceText, nameof(sourceText));

            SyntaxTree syntaxTree;
            if (this.sources.TryGetValue(sourceText, out syntaxTree))
            {
                return syntaxTree;
            }

            using (var stream = sourceText.GetStream())
                syntaxTree = SyntaxTree.CreateFromSteam(stream);
            this.sources.Add(sourceText, syntaxTree);

            return syntaxTree;
        }
    }
}