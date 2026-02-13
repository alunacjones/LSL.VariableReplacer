using System;

namespace LSL.VariableReplacer;

internal static class Guard
{
    public static T IsNotNull<T>(T value, string parameterName) => value ?? throw new ArgumentNullException(parameterName, "Argument cannot be null");

    public static T AssertNotNull<T>(this T value, string parameterName) => IsNotNull(value, parameterName);
}