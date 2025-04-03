namespace LSL.VariableReplacer;

/// <summary>
/// A source string transformer
/// </summary>
public interface ITransformer : ICanTransform
{
    /// <summary>
    /// Checks if a variable name is valid for this transformer
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    VariableNameValidationResult IsAValidVariableName(string variableName);
}