using System;
using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

internal class RegexTransformer(
    string variablePlaceholderPrefix,
    string variablePlaceholderSuffix,
    CommandProcessingDelegate commandProcessor = null) : ITransformer
{
    public RegexTransformer() : this("$(", ")", null) {}

    private readonly Lazy<Regex> _variableMatcher = new(() => new($@"{Regex.Escape(variablePlaceholderPrefix)}([\w\.]+)(:(\w+))?{Regex.Escape(variablePlaceholderSuffix)}"));

    public string Transform(IVariableResolutionContext variableResolutionContext) => 
        _variableMatcher
            .Value
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

/// <summary>
/// The type of a command processor delegate
/// </summary>
/// <param name="command">The command for the processor to use</param>
/// <param name="value">The value to be processed</param>
/// <returns></returns>
public delegate string CommandProcessingDelegate(string command, string value);