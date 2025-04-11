using System;

namespace LSL.VariableReplacer;

/// <summary>
/// Optional configuration options
/// when generating variables from environment variables
/// </summary>
public sealed class VariablesFromEnvironmentVariablesConfiguration
{
    internal Func<string, bool> EnvironmentVariableFilter { get; private set; } = key => true;
    internal bool InvalidVariableNameFilterIsDisabled { get; private set; }
    internal string Prefix { get; private set; }

    /// <summary>
    /// Sets up a filter based on the
    /// environment variable name.
    /// </summary>
    /// <param name="environmentVariableFilter"></param>
    /// <returns></returns>
    public VariablesFromEnvironmentVariablesConfiguration WithEnvironmentVariableFilter(Func<string, bool> environmentVariableFilter) => 
        this.ReturnThis(() => EnvironmentVariableFilter = Guard.IsNotNull(environmentVariableFilter, nameof(environmentVariableFilter)));

    /// <summary>
    /// Disables the automatic removal of environment
    /// variables with an invalid name.
    /// </summary>
    /// <remarks>
    /// This results in an exception being thrown if any
    /// variable names are invalid at the point of building the
    /// variable replacer.
    /// </remarks>
    /// <returns></returns>
    public VariablesFromEnvironmentVariablesConfiguration DisableInvalidVariableNameFilter() =>
        this.ReturnThis(() => InvalidVariableNameFilterIsDisabled = true);

    /// <summary>
    /// Sets the prefix that should be applied to each
    /// new variable that is added from the environment variables.
    /// </summary>
    /// <remarks>
    /// This defaults to <c>ENV_</c>
    /// </remarks>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public VariablesFromEnvironmentVariablesConfiguration WithPrefix(string prefix) => 
        this.ReturnThis(() => Prefix = prefix);
}