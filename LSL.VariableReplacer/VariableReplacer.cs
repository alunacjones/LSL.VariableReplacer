using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LSL.VariableReplacer;

internal class VariableReplacer(VariableReplacerConfiguration configuration) : IVariableReplacer
{
    public IReadOnlyDictionary<string, string> Variables => new ReadOnlyDictionary<string, string>(configuration._variables);

    public string ReplaceVariables(string sourceToReplaceVariablesIn) =>
        configuration._transformer.Transform(new VariableResolutionContext(
            sourceToReplaceVariablesIn,
            new VariableResolver(
                configuration._variables,
                configuration._whenVariableNotFound,
                new VariablePathWrapperTransformer(configuration._transformer))));

}