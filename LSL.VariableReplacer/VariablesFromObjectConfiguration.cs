using System;

namespace LSL.VariableReplacer;

/// <summary>
/// Optional configuration options
/// when generating variables from an object
/// </summary>
public sealed class VariablesFromObjectConfiguration
{
    internal string PropertyPathSeparator { get; private set; } = ".";
    internal Func<PropertyFilterContext, bool> PropertyFilter { get; private set; }
    internal string Prefix { get; private set; } = string.Empty;
    internal Func<Type, bool> PrimitiveTypeChecker { get; private set; } = type => 
        type.IsValueType ||
        type.IsPrimitive ||
        type == typeof(string) ||
        Convert.GetTypeCode(type) != TypeCode.Object;

    /// <summary>
    /// Register a property filter
    /// </summary>
    /// <param name="propertyFilter"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithPropertyFilter(Func<PropertyFilterContext, bool> propertyFilter) => 
        this.ReturnThis(() => PropertyFilter = Guard.IsNotNull(propertyFilter, nameof(propertyFilter)));

    /// <summary>
    /// Set the property path separator
    /// </summary>
    /// <param name="propertyPathSeparator"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithPropertyPathSeparator(string propertyPathSeparator) => 
        this.ReturnThis(() => PropertyPathSeparator = Guard.IsNotNull(propertyPathSeparator, nameof(propertyPathSeparator)));

    /// <summary>
    /// Sets a prefix to apply to all generated variables
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithPrefix(string prefix) =>
        this.ReturnThis(() => Prefix = Guard.IsNotNull(prefix, nameof(prefix)));

    /// <summary>
    /// Allows for a different implementation of the primitive type checking
    /// lambda if the default doesn't cover all scenarios.
    /// </summary>
    /// <remarks>
    /// The default implementation performs the following:
    /// <c>type.IsValueType || type.IsPrimitive || type == typeof(string) || Convert.GetTypeCode(type) != TypeCode.Object</c>
    /// </remarks>
    /// <param name="primitiveTypeChecker"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithPrimitiveTypeChecker(Func<Type, bool> primitiveTypeChecker) =>
        this.ReturnThis(() => PrimitiveTypeChecker = Guard.IsNotNull(primitiveTypeChecker, nameof(primitiveTypeChecker)));        
}
