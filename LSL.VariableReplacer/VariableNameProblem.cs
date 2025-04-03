namespace LSL.VariableReplacer;

/// <summary>
/// Variable name problem details container
/// </summary>
/// <param name="variableName"></param>
/// <param name="message"></param>
public class VariableNameProblem(string variableName, string message)
{
    /// <summary>
    /// The name of the problematic variable
    /// </summary>
    public string VariableName => variableName;

    /// <summary>
    /// The message about why the variable name is invalid
    /// </summary>
    public string Message => message;
}
