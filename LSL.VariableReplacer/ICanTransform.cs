namespace LSL.VariableReplacer;

/// <summary>
/// The base transform interface
/// </summary>
/// <remarks>
/// <see cref="ITransformer"/> should always be implemented as this is 
/// an internal interface
/// </remarks>
public interface ICanTransform
{
    /// <summary>
    /// Transforms the <see cref="IVariableResolutionContext.Source"/>
    /// by replacing all variables within it
    /// </summary>
    /// <param name="variableResolutionContext"></param>
    /// <returns></returns>
    string Transform(IVariableResolutionContext variableResolutionContext);
}