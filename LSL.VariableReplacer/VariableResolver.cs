using System;
using System.Collections.Generic;

namespace LSL.VariableReplacer;

internal class VariableResolver(
    IDictionary<string, string> variables,
    Func<string, string> notFoundMessageFactory,
    VariablePathWrapperTransformer transformer) : IVariableResolver
{
    public string Resolve(string variableName) => 
        variables.TryGetValue(variableName, out var value)
            ? transformer.Transform(new TrackingVariableResolutionContext(variableName, value, this))
            : notFoundMessageFactory(variableName);
}