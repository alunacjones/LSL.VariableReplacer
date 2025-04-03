using System;
using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

internal class RegexTransformer(
    string variablePlaceholderPrefix,
    string variablePlaceholderSuffix,
    CommandProcessingDelegate commandProcessor = null,
    RegexOptions? regexOptions = null,
    TimeSpan? regexTimeout = null) : ITransformer
{
    public RegexTransformer() : this("$(", ")") {}

    private static RegexOptions _defaultRegexOptions => RegexOptions.Compiled;
    private static TimeSpan _defaultRegexTimeOut => TimeSpan.FromSeconds(10);
    
    private readonly Lazy<Regex> _variableMatcher = new(() => new(
        $@"{Regex.Escape(variablePlaceholderPrefix)}([\w\.]+)(:(\w+))?{Regex.Escape(variablePlaceholderSuffix)}", 
        regexOptions ?? _defaultRegexOptions, 
        regexTimeout ?? _defaultRegexTimeOut));

    private readonly Lazy<Regex> _validVariableNameRegex = new(() => new(
        @"^[\w\.]+$",
        regexOptions ?? _defaultRegexOptions, 
        regexTimeout ?? _defaultRegexTimeOut        
    ));

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

    public VariableNameValidationResult IsAValidVariableName(string variableName) => 
        _validVariableNameRegex.Value.IsMatch(variableName)
            ? VariableNameValidationResult.Success()
            : VariableNameValidationResult.Failed($"Variable name does not conform to the regular expression of {_validVariableNameRegex.Value}");
}

/// <summary>
/// The type of a command processor delegate
/// </summary>
/// <param name="command">The command for the processor to use</param>
/// <param name="value">The value to be processed</param>
/// <returns></returns>
public delegate string CommandProcessingDelegate(string command, string value);