# Writing a Custom Transformer

The default transformer should cover most use cases for variable replacement but
there may be scenarios whereby you need full control over the parsing of a source
string and any subsequent variable replacement.

## WithTransformer

Using this method we can use a custom transformer:

```csharp { data-fiddle="ikxdMp" }
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithTransformer(new NotVeryUsefulTransformer())
        .AddVariable("name", "Als"));

var result = replacer.ReplaceVariables("Hi $name$. Where doth $name$ hail from?");

// result will be "Hi Als. Where doth Als hail from?"
```

The definition of the (aptly) named transformer is as follows:

!!! warning
    This code is not intended to be used. It is only provided to show how to interact with
    the `IVariableResolutionContext` in your own `ITransformer` implementation.

    It does not cover all edge cases and can easily break.
```csharp
internal class NotVeryUsefulTransformer : ITransformer
{
    public VariableNameValidationResult IsAValidVariableName(string variableName) => 
        variableName.Contains('$')
            ? VariableNameValidationResult.Failed(
                "The variable name cannot contain '$'"
            )
            : VariableNameValidationResult.Success();

    public string Transform(IVariableResolutionContext variableResolutionContext)
    {
        // Get the source string from the context
        var sourceString = variableResolutionContext.Source;

        // we will use this to build the replaced string
        var result = new StringBuilder();

        var index = 0;

        // Loop through each character in the string
        while (index < sourceString.Length)
        {            
            var currentCharacter = sourceString[index];

            // Check to see if we have found the start of a variable
            if (currentCharacter == '$')
            {
                // we build the name of the varaible with this
                var name = new StringBuilder();

                // Move beyond the $ start character
                index++;

                // Collect all the variable name characters
                // until we hit the closing $
                while (sourceString[index] != '$')
                {
                    name.Append(sourceString[index]);
                    index++;
                }                

                // resolve the variable value
                var variableValue = variableResolutionContext
                    .VariableResolver
                    .Resolve(name.ToString());

                // Append the value to our result StringBuilder
                result.Append(variableValue);
                index++;

                // Continue processing the string
                continue;
            }

            // Not at the start of a variable so just append the character
            result.Append(currentCharacter);

            // Move to the next character
            index++;       
        }

        // Return the variable-replaced result
        return result.ToString();
    }
}
```