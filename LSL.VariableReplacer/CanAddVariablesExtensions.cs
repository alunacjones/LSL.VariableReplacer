using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        where TSelf : ICanAddVariables<TSelf> =>
            Guard.IsNotNull(variableDictionary, nameof(variableDictionary))
                .Aggregate(source, (agg, i) => agg.AddVariable(i.Key, i.Value));

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
        where TSelf : ICanAddVariables<TSelf> => 
        source.AddEnvironmentVariables(configuration =>
        {
            configuration
                .WithEnvironmentVariableFilter(environmentVariableFilter ?? (key => true))
                .WithPrefix(prefix);
        });

    /// <summary>
    /// Adds environment variables to the variables
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <param name="source"></param>
    /// <param name="configurator">A required configurator of the environment variable adder</param>
    /// <returns></returns>
    public static TSelf AddEnvironmentVariables<TSelf>(this TSelf source, Action<VariablesFromEnvironmentVariablesConfiguration> configurator)
        where TSelf : ICanAddVariables<TSelf>
    {
        var configuration = new VariablesFromEnvironmentVariablesConfiguration();
        Guard.IsNotNull(configurator, nameof(configurator)).Invoke(configuration);
        var validateVariableName = source.GetVariableNameValidator();
        var environmentVariables = Environment.GetEnvironmentVariables();

        return environmentVariables
            .Keys
            .OfType<object>()
            .Select(k => k.ToString())
            .Where(k => configuration.InvalidVariableNameFilterIsDisabled || validateVariableName(k).Succeeded)
            .Where(configuration.EnvironmentVariableFilter)
            .Select(key => new { key, value = environmentVariables[key].ToString() })
            .Aggregate(source, (agg, i) => agg.AddVariable($"{configuration.Prefix}{i.key}", i.value));
    }

    /// <summary>
    /// Adds all properties from an object
    /// </summary>
    /// <remarks>
    /// Any object properties will be recursively traversed
    /// </remarks>
    /// <typeparam name="TSelf"></typeparam>
    /// <param name="source"></param>
    /// <param name="value"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static TSelf AddVariablesFromObject<TSelf>(this TSelf source, object value, Action<VariablesFromObjectConfiguration> configurator = null)
        where TSelf : ICanAddVariables<TSelf>
    {
        var configuration = new VariablesFromObjectConfiguration();
        configurator?.Invoke(configuration);

        return AddProperties(Guard.IsNotNull(value, nameof(value)), string.Empty);

        TSelf AddProperties(object value, string path) =>
            value.GetType()
                .GetProperties()
                .Aggregate(source, (agg, property) =>
                {
                    if (IsAPrimitiveType(property.PropertyType))
                    {
                        if (IncludeProperty()) source.AddVariable($"{configuration.Prefix}{MakePath()}", property.GetValue(value));
                        return agg;
                    }

                    AddProperties(property.GetValue(value), MakePath());
                    return agg;

                    string MakePath() =>
                        path.Length == 0 ? $"{property.Name}" : $"{path}{configuration.PropertyPathSeparator}{property.Name}";

                    bool IncludeProperty() =>
                        configuration.PropertyFilter == null || configuration.PropertyFilter(new PropertyFilterContext(property, path));
                });

        static bool IsAPrimitiveType(Type type) => type.IsPrimitive || type == typeof(string);
    }
}
