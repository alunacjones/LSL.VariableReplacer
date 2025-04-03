using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSL.VariableReplacer;

/// <summary>
/// Invalid variable names exception
/// </summary>
/// <param name="message">The message that describes the <see cref="ITransformer"/> that has issues with the variable names</param>
/// <param name="validationErrors">A collection of variable name validation errors</param>
public class InvalidVariableNamesException(string message, IEnumerable<VariableNameProblem> validationErrors) : Exception(ToMessage(message, validationErrors))    
{
    /// <summary>
    /// The variable name validation errors
    /// </summary>
    public IEnumerable<VariableNameProblem> ValidationErrors { get; } = validationErrors;

    /// <inheritdoc/>
    public static string ToMessage(string message, IEnumerable<VariableNameProblem> validationErrors)
    {
        return validationErrors.Aggregate(
            new StringBuilder(message)
                .AppendLine()
                .AppendLine(), 
            (agg, i) => agg.AppendLine($"Variable '{i.VariableName}': {i.Message}"))
            .ToString();
    }
}