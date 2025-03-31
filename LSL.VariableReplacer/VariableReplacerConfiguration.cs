using System;
using System.Collections.Generic;

namespace LSL.VariableReplacer;

/// <summary>
/// The variable replacer configuration
/// </summary>
public sealed class VariableReplacerConfiguration : ICanAddVariables<VariableReplacerConfiguration>
{
    internal IDictionary<string, object> Variables { get; } = new Dictionary<string, object>();
    internal ITransformer Transformer { get; private set; } = new RegexTransformer();
    internal Func<string, string> VariableNotFound { get; private set; } = variableName => $"NOTFOUND:{variableName}";
    internal Func<object, string> ValueFormatter { get; private set; } = value => $"{value}";
    internal Action<IDictionary<string, object>, string, object> AddToDictionaryDelelgate = (dictionary, name, value) => dictionary.Add(name, value);

    /// <inheritdoc/>
    public VariableReplacerConfiguration AddVariable(string name, object value)
    {
        Guard.IsNotNull(name, nameof(name));

        AddToDictionaryDelelgate(Variables, name, value);
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
        Guard.IsNotNull(transformer, nameof(transformer));

        Transformer = transformer;
        return this;
    }

    /// <summary>
    /// Customise the pattern used for the default Regex-based transformer
    /// </summary>
    /// <param name="variablePlaceholderPrefix"></param>
    /// <param name="variablePlaceholderSuffix"></param>
    /// <param name="commandProcessor">
    /// A delegate that accepts a <c>command</c> and a <c>value</c> paramter.
    /// Based on what the provided command is it can then return a modified version
    /// of the value if required.
    /// </param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithDefaultTransformer(
        string variablePlaceholderPrefix = "$(",
        string variablePlaceholderSuffix = ")",
        CommandProcessingDelegate commandProcessor = null) =>
        WithTransformer(new RegexTransformer(variablePlaceholderPrefix, variablePlaceholderSuffix, commandProcessor));

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
        Guard.IsNotNull(whenVariableNotFound, nameof(whenVariableNotFound));

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

    /// <summary>
    /// Optional formatter of a variable value
    /// </summary>
    /// <remarks>
    /// The default formatter will use <see cref="object.ToString"><c>ToString</c></see>
    /// to format all objects. <see langword="null"><c>null</c></see>
    /// will result in an empty string
    /// </remarks>
    /// <param name="formatter"></param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithValueFormatter(Func<object, string> formatter)
    {
        Guard.IsNotNull(formatter, nameof(formatter));

        ValueFormatter = formatter;
        return this;
    }

    /// <inheritdoc/>
    public VariableReplacerConfiguration WithAddToDictionaryDelelgate(Action<IDictionary<string, object>, string, object> action)
    {
        Guard.IsNotNull(action, nameof(action));

        AddToDictionaryDelelgate = action;
        return this;
    }

    /// <summary>
    /// Uses replace variable behaviour when
    /// adding to the variables dictionary
    /// </summary>
    /// <returns></returns>
    public VariableReplacerConfiguration WithReplaceVariableBehaviour() =>
        WithAddToDictionaryDelelgate((dicionary, name, value) => dicionary[name] = value);
}