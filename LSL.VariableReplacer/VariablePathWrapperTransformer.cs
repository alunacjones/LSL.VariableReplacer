using System;
using System.Collections.Generic;
using System.Linq;

namespace LSL.VariableReplacer;

internal class VariablePathWrapperTransformer(ITransformer transformer) : ITransformer
{
    private Stack<string> CollectedVariables { get; } = [];
    
    public string Transform(IVariableResolutionContext variableResolutionContext)
    {
        var context = (TrackingVariableResolutionContext)variableResolutionContext;
        if (CollectedVariables.Contains(context.VariableName))
        {
            var path = string.Join(" -> ", CollectedVariables.Reverse().Concat([context.VariableName]));
            throw new ArgumentException($"Cyclic dependency detected on path: {path}");
        }

        CollectedVariables.Push(context.VariableName);

        var result = transformer.Transform(variableResolutionContext);

        CollectedVariables.Pop();

        return result;
    }
}