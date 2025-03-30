namespace LSL.VariableReplacer;

/// <summary>
/// Service to resolve variables
/// </summary>
public interface IVariableResolver
{
    /// <summary>
    /// Resolves a variable to a value
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    string Resolve(string variableName);
}
