namespace LSL.VariableReplacer;

/// <summary>
/// A source string transformer
/// </summary>
public interface ITransformer
{
    /// <summary>
    /// Transforms the <see cref="IVariableResolutionContext.Source"/>
    /// by replacing all variables within it
    /// </summary>
    /// <param name="variableResolutionContext"></param>
    /// <returns></returns>
    string Transform(IVariableResolutionContext variableResolutionContext);
}