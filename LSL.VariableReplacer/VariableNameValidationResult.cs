namespace LSL.VariableReplacer;

/// <summary>
/// The result of validating a variable name for an <see cref="ITransformer"/>
/// </summary>
public class VariableNameValidationResult
{
    private readonly string _errorMessage;
    private static readonly VariableNameValidationResult _success = new(true, null);

    private VariableNameValidationResult(bool succeeded, string errorMessage)
    {
        Succeeded = succeeded;
        _errorMessage = errorMessage;
    }

    internal bool Succeeded { get; }
    
    internal string ErrorMessage => _errorMessage;

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    /// <returns></returns>
    public static VariableNameValidationResult Success() => _success;

    /// <summary>
    /// Creates a failed validation result
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static VariableNameValidationResult Failed(string errorMessage) => new(false, errorMessage);
}