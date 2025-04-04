using System.Collections.Generic;
using System.Linq;

namespace LSL.VariableReplacer;

internal class VariablePathWrapperTransformer(ITransformer transformer) : ICanTransform
{
    private Stack<string> CollectedVariables { get; } = [];

    public string Transform(IVariableResolutionContext variableResolutionContext)
    {
        var context = (TrackingVariableResolutionContext)variableResolutionContext;
        if (CollectedVariables.Contains(context.VariableName))
        {
            throw new CyclicDependencyException(CollectedVariables.Reverse().Concat([context.VariableName]));
        }

        CollectedVariables.Push(context.VariableName);

        var result = transformer.Transform(variableResolutionContext);

        CollectedVariables.Pop();

        return result;
    }
}