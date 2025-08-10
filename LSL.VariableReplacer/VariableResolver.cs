namespace LSL.VariableReplacer;

internal class VariableResolver(VariableReplacerConfiguration config, VariablePathWrapperTransformer transformer) : IVariableResolver
{
    public string Resolve(string variableName)
    {
        return config.VariableResolver(config.ReadOnlyVariables, variableName, out var value) 
            ? Transform(value) 
            : config.VariableNotFound(variableName);

        string Transform(object value) => 
            transformer.Transform(new TrackingVariableResolutionContext(variableName, config.ValueFormatter(value), this));
    }
}