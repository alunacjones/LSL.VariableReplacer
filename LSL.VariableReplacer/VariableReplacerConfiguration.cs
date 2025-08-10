using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LSL.VariableReplacer;

/// <summary>
/// The variable replacer configuration
/// </summary>
public sealed class VariableReplacerConfiguration : ICanAddVariables<VariableReplacerConfiguration>, IHaveATransformer
{
    internal VariableReplacerConfiguration() : this(
        new Dictionary<string, object>(),
        new RegexTransformer(),
        variableName => $"NOTFOUND:{variableName}",
        value => $"{value}",
        DefaultAddToDictionaryBehaviour
    ) { }

    private static void DefaultAddToDictionaryBehaviour(IDictionary<string, object> dictionary, string name, object value) =>
        dictionary.Add(name, value);

    private static bool DefaultVariableResolver(IReadOnlyDictionary<string, object> dictionary, string variableName, out object result) =>
        dictionary.TryGetValue(variableName, out result);

    internal VariableReplacerConfiguration(
        IDictionary<string, object> variables,
        ITransformer transformer,
        Func<string, string> variableNotFound,
        Func<object, string> valueFormatter,
        Action<IDictionary<string, object>, string, object> addToDictionaryDelegate)
    {
        Variables = variables;
        ReadOnlyVariables = new ReadOnlyDictionary<string, object>(Variables);
        Transformer = transformer;
        VariableNotFound = variableNotFound;
        ValueFormatter = valueFormatter;
        AddToDictionaryDelegate = addToDictionaryDelegate;
    }

    internal IDictionary<string, object> Variables { get; }
    internal IReadOnlyDictionary<string, object> ReadOnlyVariables { get; }
    internal ITransformer Transformer { get; private set; }
    internal Func<string, string> VariableNotFound { get; private set; }
    internal Func<object, string> ValueFormatter { get; private set; }
    internal Action<IDictionary<string, object>, string, object> AddToDictionaryDelegate { get; private set; }
    internal VariableResolvingDelegate VariableResolver = DefaultVariableResolver;

    ITransformer IHaveATransformer.Transformer => Transformer;

    /// <inheritdoc/>
    public VariableReplacerConfiguration AddVariable(string name, object value) =>
        this.ReturnThis(() => AddToDictionaryDelegate(Variables, Guard.IsNotNull(name, nameof(name)), value));

    /// <summary>
    /// Use a custom transformer
    /// </summary>
    /// <remarks>
    /// The default transformer uses a <see cref="Regex"/>
    /// that matches variables of the form <c>$(VariableName)</c>
    /// </remarks>
    /// <param name="transformer">A custom transformer</param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithTransformer(ITransformer transformer) =>
        this.ReturnThis(() => Transformer = Guard.IsNotNull(transformer, nameof(transformer)));

    /// <summary>
    /// Customise the Regex-based transformer
    /// </summary>
    /// <param name="variablePlaceholderPrefix"></param>
    /// <param name="variablePlaceholderSuffix"></param>
    /// <param name="commandProcessor">
    /// A delegate that accepts a <c>command</c> and a <c>value</c> parameter.
    /// Based on what the provided command is it can then return a modified version
    /// of the value if required.
    /// </param>
    /// <param name="regexOptions">
    /// The <see cref="RegexOptions"/> to configure the transformer with.
    /// If not set then <c>RegexOptions.Compiled</c> will be used
    /// </param>
    /// <param name="regexTimeOut">
    /// The timeout for the regular expression.
    /// If not set then it defaults to 10 seconds
    /// </param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithDefaultTransformer(
        string variablePlaceholderPrefix = "$(",
        string variablePlaceholderSuffix = ")",
        CommandProcessingDelegate commandProcessor = null,
        RegexOptions? regexOptions = null,
        TimeSpan? regexTimeOut = null) =>
        WithTransformer(new RegexTransformer(
            variablePlaceholderPrefix,
            variablePlaceholderSuffix,
            commandProcessor,
            regexOptions,
            regexTimeOut));

    /// <summary>
    /// Provide a custom replacement string
    /// for a variable placeholder if it is not found
    /// </summary>
    /// <remarks>
    /// An exception can be thrown instead of returning a string.
    /// The string passed into the function is the name of the variable.
    /// </remarks>
    /// <param name="whenVariableNotFound"></param>
    /// <returns></returns>
    public VariableReplacerConfiguration WhenVariableNotFound(Func<string, string> whenVariableNotFound) =>
        this.ReturnThis(() => VariableNotFound = Guard.IsNotNull(whenVariableNotFound, nameof(whenVariableNotFound)));

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
    public VariableReplacerConfiguration WithValueFormatter(Func<object, string> formatter) =>
        this.ReturnThis(() => ValueFormatter = Guard.IsNotNull(formatter, nameof(formatter)));

    /// <inheritdoc/>
    public VariableReplacerConfiguration WithAddToDictionaryDelegate(Action<IDictionary<string, object>, string, object> action) =>
        this.ReturnThis(() => AddToDictionaryDelegate = Guard.IsNotNull(action, nameof(action)));

    /// <summary>
    /// Uses replace variable behaviour when
    /// adding to the variables dictionary
    /// </summary>
    /// <returns></returns>
    public VariableReplacerConfiguration WithReplaceVariableBehaviour() =>
        WithAddToDictionaryDelegate((dictionary, name, value) => dictionary[name] = value);


    /// <summary>
    /// Changes the variable 'add' behaviour to not allow duplicates
    /// </summary>
    /// <remarks>
    /// The is the default behaviour
    /// </remarks>
    /// <returns></returns>
    public VariableReplacerConfiguration WithNoDuplicateAddVariableBehaviour() =>
        WithAddToDictionaryDelegate(DefaultAddToDictionaryBehaviour);

    /// <summary>
    /// Use a custom resolver to fetch a value from the variable
    /// replacer's dictionary
    /// </summary>
    /// <param name="resolver"></param>
    /// <returns></returns>
    public VariableReplacerConfiguration WithVariableResolver(VariableResolvingDelegate resolver)
    {
        VariableResolver = resolver;
        return this;
    }

    internal VariableReplacerConfiguration Clone(Func<IDictionary<string, object>, IDictionary<string, object>> dictionaryCloner = null) =>
        new((dictionaryCloner ?? CopyDictionary)(Variables),
            Transformer,
            VariableNotFound,
            ValueFormatter,
            AddToDictionaryDelegate);

    private static IDictionary<string, object> CopyDictionary(IDictionary<string, object> source) =>
        source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
}