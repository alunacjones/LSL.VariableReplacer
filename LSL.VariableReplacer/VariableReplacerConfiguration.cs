using System;
using System.Collections.Generic;

namespace LSL.VariableReplacer;

/// <summary>
/// The variable replacer configuration
/// </summary>
public sealed class VariableReplacerConfiguration : ICanAddVariables<VariableReplacerConfiguration>
{
    internal IDictionary<string, string> Variables { get; } = new Dictionary<string, string>();
    internal ITransformer Transformer { get; private set; } = new RegexTransformer();
    internal Func<string, string> VariableNotFound { get; private set; } = variableName => $"NOTFOUND:{variableName}";

    /// <inheritdoc/>
    public VariableReplacerConfiguration AddVariable(string name, string value)
    {
        Guard.IsNotNull(name, nameof(name));

        Variables.Add(name, value);
        return this;
    }

    /// <summary>
    /// Use a custom transformer
    /// </summary>
    /// <remarks>
    /// The default transformer uses a <see cref="System.Text.RegularExpressions.Regex"/>
    /// That matches variables of the form <c>$(VariableName)</c>
    /// </remarks>
    /// <param name="transformer">A custom transformer</param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithTransformer(ITransformer transformer)
    {
         Transformer = transformer;
         return this;
    }

    /// <summary>
    /// Customise the pattern used for the default Regex-based transformer
    /// </summary>
    /// <param name="variablePlaceholderPrefix"></param>
    /// <param name="variablePlaceholderSuffix"></param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithDefaultTransformer(string variablePlaceholderPrefix, string variablePlaceholderSuffix) =>
        WithTransformer(new RegexTransformer(variablePlaceholderPrefix, variablePlaceholderSuffix));

    /// <summary>
    /// Provide a custom replacement string
    /// for a variable placeholder if it is not found
    /// </summary>
    /// <remarks>
    /// An exception can be thrown instead of returning a string
    /// </remarks>
    /// <param name="whenVariableNotFound"></param>
    /// <returns></returns>
    public VariableReplacerConfiguration WhenVariableNotFound(Func<string, string> whenVariableNotFound)
    {
        VariableNotFound = whenVariableNotFound;
        return this;
    }

    /// <summary>
    /// Throw an exception if a variable is not found
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public VariableReplacerConfiguration ThrowIfVariableNotFound() => 
        WhenVariableNotFound(variableName => throw new ArgumentException($"Variable '{variableName}' not found"));
}