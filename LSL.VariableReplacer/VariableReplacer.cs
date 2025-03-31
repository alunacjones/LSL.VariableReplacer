using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LSL.VariableReplacer;

internal class VariableReplacer(VariableReplacerConfiguration configuration) : IVariableReplacer
{
    public IReadOnlyDictionary<string, object> Variables => new ReadOnlyDictionary<string, object>(configuration.Variables);

    public string ReplaceVariables(string sourceToReplaceVariablesIn) =>
        configuration.Transformer.Transform(new VariableResolutionContext(
            sourceToReplaceVariablesIn,
            new VariableResolver(
                configuration,
                new VariablePathWrapperTransformer(configuration.Transformer)
            )
        ));
}