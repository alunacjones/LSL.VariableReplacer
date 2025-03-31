using System;
using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

internal class RegexTransformer(
    string variablePlaceholderPrefix,
    string variablePlaceholderSuffix,
    Func<string, string, string> commandProcessor = null) : ITransformer
{
    public RegexTransformer() : this("$(", ")", null) {}

    internal Regex VariableMatcher => new($@"{Regex.Escape(variablePlaceholderPrefix)}([\w\.]+)(:(\w+))?{Regex.Escape(variablePlaceholderSuffix)}");

    public string Transform(IVariableResolutionContext variableResolutionContext) => 
        VariableMatcher
            .Replace(
                variableResolutionContext.Source, 
                m => 
                {
                    commandProcessor ??= (command, value) => value;
                    var command = m.Groups[3].Value;

                    return commandProcessor(
                        command, 
                        variableResolutionContext.VariableResolver.Resolve(m.Groups[1].Value));
                });
}