using System;

namespace LSL.VariableReplacer;

/// <summary>
/// Variable Replacer Extensions
/// </summary>
public static class VariableReplacerExtensions
{
    /// <summary>
    /// Clones an existing variable replacer and allows for
    /// further configuration of the clone via the <paramref name="configurator"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IVariableReplacer CloneAndConfigure(this IVariableReplacer source, Action<VariableReplacerConfiguration> configurator)
    {
        Guard.IsNotNull(source, nameof(source));
        Guard.IsNotNull(configurator, nameof(configurator));

        return VariableReplacerFactory.InnerBuild(((VariableReplacer)source).Configuration.Clone(), configurator);
    }
}