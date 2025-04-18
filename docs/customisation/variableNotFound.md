# When a Variable is not found

The default behaviour for when a variable is not found is to replace the variable
placeholder with a string that indicates this.

For example, if the variable `name` has not been added then a source string of `Hi $(name)`
would result in a value of `Hi NOTFOUND:name`

The following examples show how to customise this behaviour.

## WhenVariableNotFound

This method can be used to customise the message returned.

!!! note
    Your method could throw an exception rather than return a replacement

```csharp { data-fiddle="M0mR8V" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.WhenVariableNotFound(variableName => $"ERROR:{variableName}"));

var result = replacer.ReplaceVariables("Hello $(Unknown)");

// result will be "Hello ERROR:Unknown"
```

## ThrowIfVariableNotFound

This method will ensure an `ArgumentException` is thrown if a variable is not found.
The message of the exception would be `Variable 'Unknown' not found` when the missing variable
is called `Unknown`.

The following example shows how to set it up:

```csharp { data-fiddle="sv6mtC" }
var sut = new VariableReplacerFactory()
    .Build(c => c.ThrowIfVariableNotFound());

// The following code will throw an ArgumentException
var result = sut.ReplaceVariables("Hello $(Unknown)");
```