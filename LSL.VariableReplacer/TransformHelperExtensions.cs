using System;

namespace LSL.VariableReplacer;

internal static class TransformHelperExtensions
{
    public static Func<string, VariableNameValidationResult> GetVariableNameValidator(this object source) => 
        source.TryAs<IHaveATransformer>(out var transformerContainer)
            ? transformerContainer.Transformer.IsAValidVariableName
            : new Func<string, VariableNameValidationResult>(static key => VariableNameValidationResult.Success());

    public static bool TryAs<T>(this object source, out T result)
        where T : class => 
        (result = source as T) is not null;
}