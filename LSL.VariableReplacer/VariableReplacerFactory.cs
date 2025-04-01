using System;

namespace LSL.VariableReplacer;

/// <summary>
/// A factory for creating a variable replacer
/// </summary>
public class VariableReplacerFactory
{
    /// <summary>
    /// Builds a variable replacer using the provided
    /// configurator
    /// </summary>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public IVariableReplacer Build(Action<VariableReplacerConfiguration> configurator)
    {
        Guard.IsNotNull(configurator, nameof(configurator));

        return InnerBuild(new VariableReplacerConfiguration(), configurator);
    }

    internal static IVariableReplacer InnerBuild(VariableReplacerConfiguration configuration, Action<VariableReplacerConfiguration> configurator)
    {
        configurator.Invoke(configuration);

        return new VariableReplacer(configuration);
    }
}
