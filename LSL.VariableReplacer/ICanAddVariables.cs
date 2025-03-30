namespace LSL.VariableReplacer;

/// <summary>
/// Variable adding interface
/// </summary>
/// <typeparam name="TSelf"></typeparam>
public interface ICanAddVariables<TSelf>
{
    /// <summary>
    /// Adds a variable to the variable replacer
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    TSelf AddVariable(string name, string value);    
}