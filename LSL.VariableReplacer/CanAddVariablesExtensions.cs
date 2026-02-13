using System;
using System.Collections;
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
                    if (IncludeProperty() is false) return agg;

                    var propertyValue = property.GetValue(value);
                    
                    if (propertyValue is IEnumerable enumerable && configuration.EnumerableTypeChecker(property.PropertyType))
                    {
                        IterateEnumerable(enumerable);
                        return agg;
                    }

                    AddVariable(propertyValue, property.PropertyType);

                    return agg;

                    string MakePath(string suffix = null) =>
                        path.Length == 0 ? $"{property.Name}{suffix}" : $"{path}{configuration.PropertyPathSeparator}{property.Name}";

                    void AddVariable(object value, Type valueType, string pathSuffix = null)
                    {
                        var path = $"{configuration.Prefix}{MakePath()}{pathSuffix}";

                        if (configuration.PrimitiveTypeChecker(valueType) || value is null)
                        {
                            source.AddVariable(path, value);
                            return;
                        }

                        AddProperties(value, MakePath(pathSuffix));
                    }

                    void IterateEnumerable(IEnumerable enumerable)
                    {
                        var index = 0;
                        foreach (var item in enumerable)
                        {
                            var itemType = item.GetType();
                            var suffix = $"{configuration.ArrayIndexDelimiters[0]}{index}{configuration.ArrayIndexDelimiters[1]}";
                            var path = MakePath($"{configuration.ArrayIndexDelimiters[0]}{index}{configuration.ArrayIndexDelimiters[1]}");
                            AddVariable(item, itemType, suffix);

                            index++;
                        }
                    }
                    bool IncludeProperty() =>
                       configuration.PropertyFilter == null || configuration.PropertyFilter(new PropertyFilterContext(property, path));
                });
    }
}
