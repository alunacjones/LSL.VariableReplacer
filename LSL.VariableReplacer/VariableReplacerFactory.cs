using System;

namespace LSL.VariableReplacer;

/// <summary>
/// A factory for creating a variable replacer
/// </summary>
public class VariableReplacerFactory : IVariableReplacerFactory
{
    /// <inheritdoc/>
    public IVariableReplacer Build(Action<VariableReplacerConfiguration> configurator) => 
        InnerBuild(new VariableReplacerConfiguration(), Guard.IsNotNull(configurator, nameof(configurator)));

    internal static IVariableReplacer InnerBuild(VariableReplacerConfiguration configuration, Action<VariableReplacerConfiguration> configurator)
    {
        configurator.Invoke(configuration);

        return new VariableReplacer(configuration);
    }
}
