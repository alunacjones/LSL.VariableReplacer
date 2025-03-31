using System;
using System.Collections.Generic;

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
    TSelf AddVariable(string name, object value);    

    /// <summary>
    /// Provide a method for adding to the variables dictionary.
    /// </summary>
    /// <remarks>
    /// The default behaviour is to use <see cref="IDictionary{TKey, TValue}.Add"><c>Add</c></see>.
    /// This default behaviour raises an exception if a duplicate is added
    /// </remarks>    
    TSelf WithAddToDictionaryDelelgate(Action<IDictionary<string, object>, string, object> action);
}