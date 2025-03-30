using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

internal class RegexTransformer(string variablePlaceholderPrefix, string variablePlaceholderSuffix) : ITransformer
{
    public RegexTransformer() : this("$(", ")") {}

    internal Regex VariableMatcher { get; set; } = new Regex($@"{Regex.Escape(variablePlaceholderPrefix)}(\w+){Regex.Escape(variablePlaceholderSuffix)}");

    public string Transform(IVariableResolutionContext variableResolutionContext) => 
        VariableMatcher
            .Replace(
                variableResolutionContext.Source, 
                m => variableResolutionContext.VariableResolver.Resolve(m.Groups[1].Value));
}