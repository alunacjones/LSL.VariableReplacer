namespace LSL.VariableReplacer;

internal class TrackingVariableResolutionContext(string variableName, string source, IVariableResolver variableResolver) : VariableResolutionContext(source, variableResolver)
{
    public string VariableName { get; } = variableName;
}
