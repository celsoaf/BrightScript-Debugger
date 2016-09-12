using System.Collections.Generic;
using BrightScript.Language.Formatting.Options;
using BrightScript.Language.Shared;
using BrightScript.Language.Text;
using Microsoft;

namespace BrightScript.Language.Formatting
{
    public sealed class Formatter
    {
        private FormattingOptions formattingOptions;
        
        /// <summary>
        /// This is main entry point for the VS side of things. For now, the implementation
        /// of the function is not final and it just used as a way seeing results in VS.
        /// Ideally, Format will also take in a "formatting option" object that dictates
        /// the rules that should be enabled, spacing and tabs.
        /// </summary>
        /// <param name="sourceText">The SourceText that represents the text to be formatted</param>
        /// <param name="range">The range of indicies to be formatted</param>
        /// <param name="formattingOptions">The options to format with, null leaves the options as they were</param>
        /// <returns>
        /// A list of TextEditInfo objects are returned for the spacing between tokens (starting from the
        /// first token in the document to the last token. After the spacing text edits, the indentation
        /// text edits follow (starting again from the beginning of the document). I might separate the
        /// indentation text edits from the spacing text edits in the future but for now they are in
        /// the same list.
        /// </returns>
        public List<TextEditInfo> Format(SourceText sourceText, Range range, FormattingOptions formattingOptions)
        {
            Requires.NotNull(formattingOptions, nameof(formattingOptions));
            Requires.NotNull(sourceText, nameof(sourceText));

            this.formattingOptions = formattingOptions;

            List<TextEditInfo> textEdits = new List<TextEditInfo>();

            textEdits.Sort((x, y) => x.Start < y.Start ? 1 : x.Start == y.Start ? 0 : -1);

            return textEdits;
        }
    }

}