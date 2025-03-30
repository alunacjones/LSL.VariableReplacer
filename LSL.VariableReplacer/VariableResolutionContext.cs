namespace LSL.VariableReplacer;

internal class VariableResolutionContext(string source, IVariableResolver variableResolver) : IVariableResolutionContext
{
    public string Source => source;
    public IVariableResolver VariableResolver => variableResolver;
}
