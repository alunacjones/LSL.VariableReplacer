using System;
using System.Collections.Generic;
using System.Linq;

namespace LSL.VariableReplacer;

/// <summary>
/// The variable replacer configuration
/// </summary>
public sealed class VariableReplacerConfiguration : ICanAddVariables<VariableReplacerConfiguration>
{
    internal VariableReplacerConfiguration(): this(
        new Dictionary<string, object>(),
        new RegexTransformer(),
        variableName => $"NOTFOUND:{variableName}",
        value => $"{value}",
        (dictionary, name, value) => dictionary.Add(name, value)
    ) {}

    internal VariableReplacerConfiguration(
        IDictionary<string, object> variables,
        ITransformer transformer,
        Func<string, string> variableNotFound,
        Func<object, string> valueFormatter,
        Action<IDictionary<string, object>, string, object> addToDictionaryDelelgate)
    {
        Variables = variables;
        Transformer = transformer;
        VariableNotFound = variableNotFound;
        ValueFormatter = valueFormatter;
        AddToDictionaryDelelgate = addToDictionaryDelelgate;
    }

    internal IDictionary<string, object> Variables { get; } 
    internal ITransformer Transformer { get; private set; }
    internal Func<string, string> VariableNotFound { get; private set; }
    internal Func<object, string> ValueFormatter { get; private set; }
    internal Action<IDictionary<string, object>, string, object> AddToDictionaryDelelgate { get; private set; }

    /// <inheritdoc/>
    public VariableReplacerConfiguration AddVariable(string name, object value)
    {
        AddToDictionaryDelelgate(Variables, Guard.IsNotNull(name, nameof(name)), value);
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
        Transformer = Guard.IsNotNull(transformer, nameof(transformer));
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
        VariableNotFound = Guard.IsNotNull(whenVariableNotFound, nameof(whenVariableNotFound));
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
        ValueFormatter = Guard.IsNotNull(formatter, nameof(formatter));
        return this;
    }

    /// <inheritdoc/>
    public VariableReplacerConfiguration WithAddToDictionaryDelelgate(Action<IDictionary<string, object>, string, object> action)
    {        
        AddToDictionaryDelelgate = Guard.IsNotNull(action, nameof(action));
        return this;
    }

    /// <summary>
    /// Uses replace variable behaviour when
    /// adding to the variables dictionary
    /// </summary>
    /// <returns></returns>
    public VariableReplacerConfiguration WithReplaceVariableBehaviour() =>
        WithAddToDictionaryDelelgate((dicionary, name, value) => dicionary[name] = value);

    internal VariableReplacerConfiguration Clone() =>
        new(CopyDictionary(Variables), Transformer, VariableNotFound, ValueFormatter, AddToDictionaryDelelgate);

    private static IDictionary<string, object> CopyDictionary(IDictionary<string, object> source) =>
        source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}