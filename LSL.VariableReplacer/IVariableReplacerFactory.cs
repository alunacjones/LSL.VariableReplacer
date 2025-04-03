using System;

namespace LSL.VariableReplacer;

/// <summary>
/// The interface for the variable replacer factory
/// </summary>
public interface IVariableReplacerFactory
{
    /// <summary>
    /// Builds a variable replacer using the provided
    /// configurator
    /// </summary>
    /// <param name="configurator"></param>
    /// <returns></returns>    
    IVariableReplacer Build(Action<VariableReplacerConfiguration> configurator);
}