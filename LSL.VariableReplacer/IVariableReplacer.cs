using System.Collections.Generic;

namespace LSL.VariableReplacer;

/// <summary>
/// A variable replacer
/// </summary>
public interface IVariableReplacer
{
    /// <summary>
    /// Replaces variables in <paramref name="sourceToReplaceVariablesIn"/>
    /// </summary>
    /// <param name="sourceToReplaceVariablesIn"></param>
    /// <returns></returns>
    string ReplaceVariables(string sourceToReplaceVariablesIn);

    /// <summary>
    /// The variables available to replace with
    /// </summary>
    IReadOnlyDictionary<string, object> Variables { get;}
}