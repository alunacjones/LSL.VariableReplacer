using System.Collections.Generic;

namespace LSL.VariableReplacer;

/// <summary>
/// Variable resolving delegate
/// </summary>
/// <param name="variables"></param>
/// <param name="variableName"></param>
/// <param name="result"></param>
/// <returns></returns>
public delegate bool VariableResolvingDelegate(IReadOnlyDictionary<string, object> variables, string variableName, out object result);