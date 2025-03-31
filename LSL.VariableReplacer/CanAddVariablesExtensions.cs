using System;
using System.Collections.Generic;
using System.Linq;

namespace LSL.VariableReplacer;

/// <summary>
/// Extensions for ICanAddVariables
/// </summary>
public static class CanAddVariablesExtensions
{
    /// <summary>
    /// Adds a dictionary of variables
    /// to the configuration
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <param name="source"></param>
    /// <param name="variableDictionary"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static TSelf AddVariables<TSelf>(this TSelf source, IDictionary<string, object> variableDictionary)
        where TSelf : ICanAddVariables<TSelf>
    {
        Guard.IsNotNull(variableDictionary, nameof(variableDictionary));
        
        return variableDictionary.Aggregate(source, (agg, i) => agg.AddVariable(i.Key, i.Value));
    }

    /// <summary>
    /// Adds environment variables to the variables
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <param name="source"></param>
    /// <param name="environmentVariableFilter">
    /// An optional filter that is given the envionment variable
    /// key and returns <c>true</c> if it should be included
    /// </param>
    /// <param name="prefix">
    /// A prefix to apply to each variable's key. 
    /// The resultant key is <c>Prefix + Environment Variable Key</c>
    /// </param>
    /// <returns></returns>
    public static TSelf AddEnvironmentVariables<TSelf>(this TSelf source, Func<string, bool> environmentVariableFilter = null, string prefix = "ENV_")
        where TSelf : ICanAddVariables<TSelf>
    {
        environmentVariableFilter ??= key => true;
        var environmentVariables = Environment.GetEnvironmentVariables();

        return environmentVariables
            .Keys
            .OfType<object>()
            .Select(k => k.ToString())
            .Where(environmentVariableFilter)
            .Select(key => new { key, value = environmentVariables[key].ToString() })
            .Aggregate(source, (agg, i) => agg.AddVariable($"{prefix}{i.key}", i.value));
    }
}
