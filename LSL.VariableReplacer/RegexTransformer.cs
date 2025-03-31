using System;
using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

internal class RegexTransformer(
    string variablePlaceholderPrefix,
    string variablePlaceholderSuffix,
    Func<string, (string, Func<string, string>)> keyPreprocessor) : ITransformer
{
    public RegexTransformer() : this("$(", ")", key => (key, v => v)) {}

    internal Regex VariableMatcher => new($@"{Regex.Escape(variablePlaceholderPrefix)}([\w:]+){Regex.Escape(variablePlaceholderSuffix)}");

    public string Transform(IVariableResolutionContext variableResolutionContext) => 
        VariableMatcher
            .Replace(
                variableResolutionContext.Source, 
                m => { 
                    var (key, transform) = keyPreprocessor(m.Groups[1].Value);
                    return transform(variableResolutionContext.VariableResolver.Resolve(key));
                });
}