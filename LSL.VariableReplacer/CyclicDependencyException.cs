using System;
using System.Collections.Generic;

namespace LSL.VariableReplacer;

/// <summary>
/// Thrown when a cyclic dependency is detected
/// </summary>
public class CyclicDependencyException(IEnumerable<string> elements) : Exception(ToMessage(elements))
{
    /// <summary>
    /// The list of elements that depict the cyclic dependency
    /// </summary>
    public IEnumerable<string> Elements => elements;

    internal static string ToMessage(IEnumerable<string> paths) => 
        $"Cyclic dependency detected on path: {string.Join(" -> ", paths)}";
}