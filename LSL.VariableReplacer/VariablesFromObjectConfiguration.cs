using System;
using System.Collections;

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
    internal string[] ArrayIndexDelimiters { get; set; } = [".", string.Empty];

    internal Func<Type, bool> PrimitiveTypeChecker { get; private set; } = type => 
        type.IsValueType ||
        type.IsPrimitive ||
        type == typeof(string) ||
        Convert.GetTypeCode(type) != TypeCode.Object;

    private static readonly Type _stringType = typeof(string);

    internal Func<Type, bool> EnumerableTypeChecker { get; private set; } = type =>
        type != _stringType;

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

    /// <summary>
    /// Set up custom array index prefix and suffix values
    /// </summary>
    /// <remarks>
    /// <para>
    ///     The default transformer's regex uses only the prefix as a <c>.</c> due to its
    ///     variable name format.
    /// </para>
    /// <para>
    ///     With the defaults, an enumerable string property called <c>StringArray</c> with a single item
    ///     <c>first</c> will produce a variable name of <c>StringArray.0</c>
    /// </para>
    /// </remarks>
    /// <param name="indexPrefix"></param>
    /// <param name="indexSuffix"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithArrayIndexDelimiters(string indexPrefix, string indexSuffix) =>
        this.ReturnThis(() => ArrayIndexDelimiters = [
            indexPrefix.AssertNotNull(nameof(indexPrefix)), 
            indexSuffix.AssertNotNull(nameof(indexSuffix))]);

    /// <summary>
    /// Setup a custom checker of a type to assert that it is definitely an enumerable
    /// </summary>
    /// <remarks>
    /// The default implementation ensures that the type is <b>not</b> a <see langword="string"/>.
    /// An initial check is already done prior to this check as to whether the type is an
    /// <see cref="IEnumerable"/>
    /// </remarks>
    /// <param name="enumerableTypeChecker"></param>
    /// <returns></returns>
    public VariablesFromObjectConfiguration WithEnumerableChecker(Func<Type, bool> enumerableTypeChecker) =>
        this.ReturnThis(() => EnumerableTypeChecker = enumerableTypeChecker.AssertNotNull(nameof(enumerableTypeChecker)));
}
