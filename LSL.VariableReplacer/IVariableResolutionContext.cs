namespace LSL.VariableReplacer;

/// <summary>
/// The variable resolution context
/// </summary>
public interface IVariableResolutionContext
{
    /// <summary>
    /// The source string that has variable
    /// place holders that need replacing with
    /// real values
    /// </summary>
    string Source { get; }

    /// <summary>
    /// The variable resolver
    /// </summary>
    IVariableResolver VariableResolver { get;}
}
