using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LSL.VariableReplacer;

internal class VariableReplacer(VariableReplacerConfiguration configuration) : IVariableReplacer
{
    public IReadOnlyDictionary<string, string> Variables => new ReadOnlyDictionary<string, string>(configuration.Variables);

    public string ReplaceVariables(string sourceToReplaceVariablesIn) =>
        configuration.Transformer.Transform(new VariableResolutionContext(
            sourceToReplaceVariablesIn,
            new VariableResolver(
                configuration.Variables,
                configuration.VariableNotFound,
                new VariablePathWrapperTransformer(configuration.Transformer))));

}