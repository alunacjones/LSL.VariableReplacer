using System;

namespace LSL.VariableReplacer;

internal static class ReturnSelfExtensions
{
    public static TSelf ReturnThis<TSelf>(this TSelf source, Action toRun)
    {
        Guard.IsNotNull(toRun, nameof(toRun)).Invoke();
        return source;
    }
}