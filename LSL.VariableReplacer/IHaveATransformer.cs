namespace LSL.VariableReplacer;

internal interface IHaveATransformer
{
    ITransformer Transformer { get; }
}