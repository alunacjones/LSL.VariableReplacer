using System.Reflection;

namespace LSL.VariableReplacer;

/// <summary>
/// The property filter context
/// </summary>
public readonly struct PropertyFilterContext
{
    internal PropertyFilterContext(PropertyInfo property, string parentPath)
    {
        Property = property;
        ParentPath = parentPath;
    }
    
    /// <summary>
    /// The property to test if it can be included
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// The parent path of the current property
    /// </summary>
    public string ParentPath { get; }
}