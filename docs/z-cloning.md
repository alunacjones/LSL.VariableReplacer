---
tags:
    - partial
    - clone
    - cloning
    - inheritance
---

# Cloning and Rebuilding

Sometimes it may be beneficial to have a base `IVariableReplacer` that
can be further customised later on in the execution of your application.

This can be seen as utilising inheritance or even a partial definition of a variable replacer.

In reality an `IVariableReplacer` is immutable so this is where the s`CloneAndConfigure` extension method 
of an `IVariableReplacer` comes into play:

!!! note
    The cloning does a shallow clone of the variables and copies
    all the other settings across from the original `IVariableReplacer`

```csharp
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariables(new Dictionary<string, object>
    {
        ["FirstName"] = "Al",
        ["LastName"] = "Jones",
        ["FullName"] = "$(FirstName) $(LastName)"
    }));

var result = replacer.ReplaceVariables(
    "Hello $(FullName). Can I call you $(FirstName)?"
);
// result should be "Hello Al Jones. Can I call you Al?"

// We configure the second replacer to allow us to replace existing variables
// with the `WithReplaceVariableBehaviour` method below
var replacer2 = replacer.CloneAndConfigure(c => c
    .WithReplaceVariableBehaviour()
    .AddVariable("FirstName", "Other"));

var result2 = replacer2.ReplaceVariables(
    "Hello $(FullName). Can I call you $(FirstName)?"
);
// result2 should be "Hello Other Jones. Can I call you Other?"

var firstName = replacer.Variables["FirstName"];
// firstName should be "Al" as the clone will 
// ensure that the initial replacer is preserved

var firstName2 = replacer2.Variables["FirstName"];
// firstName2 should be "Other"
```
