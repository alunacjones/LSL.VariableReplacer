---
tags:
    - variables
    - cycle
---
# Variables Within Variables

Variables can reference other variables. This allows for composing of
your data and preventing repeated code.

## An example

The following code shows us setting up a `FirstName` variable and a `LastName` variable.
We also setup a `FullName` variable that uses both `FirstName` and `LastName` to resolve its
content:

```csharp { data-fiddle="DA3Lku" }
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
// result will be "Hello Al Jones. Can I call you Al?"
```

## Cyclic References

When resolving a variable, if a cyclic dependency is detected then
an `ArgumentExceptoion` is hghlighting the cyclic path.

This example shows this in effect:

```csharp { data-fiddle="UtvWiS" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariables(new Dictionary<string, object>
    {
        ["FirstName"] = "Al",
        ["LastName"] = "Jones",
        ["FullName"] = "$(FirstName) $(LastName) $(Other)",
        ["Other"] = "Stuff $(Another)",
        ["Another"] = "$(FullName)"
    }));

// This wil throw an exception
var result =  replacer.ReplaceVariables("Hello $(FullName). Can I call you $(FirstName)?");
```