using System;
using System.Collections.Generic;
using System.Linq;

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

        var problems = configuration.Variables
            .Select(variable => new { variable,  validationResult = configuration.Transformer.IsAValidVariableName(variable.Key) })
            .Aggregate(new List<VariableNameProblem>(), (agg, i) =>
            {
                if (!i.validationResult.Succeeded)
                {
                    agg.Add(new VariableNameProblem(i.variable.Key, i.validationResult.ErrorMessage));
                }
                
                return agg;
            });

        if (problems.Any())
        {
            throw new InvalidVariableNamesException(
                $"There are variable names that do not conform to the convention for {configuration.Transformer.GetType().FullName}",
                problems);
        }

        return new VariableReplacer(configuration);
    }
}