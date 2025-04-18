---
tags:
    - partial
    - clone
    - cloning
    - inheritance
---

# Cloning and Rebuilding

## Using the default dictionary cloner

Sometimes it may be beneficial to have a base `IVariableReplacer` that
can be further customised later on in the execution of your application.

This can be seen as utilising inheritance or even a partial definition of a variable replacer.

In reality an `IVariableReplacer` is immutable so this is where the `CloneAndConfigure` extension method 
of an `IVariableReplacer` comes into play:

!!! note
    The cloning does a shallow clone of the variables and copies
    all the other settings across from the original `IVariableReplacer`.
    A custom cloning function can be passed in as an extra
    argument as demonstrated [here](#using-the-a-custom-dictionary-cloner)

```csharp { data-fiddle="qgWwVy" }
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

// We configure the second replacer to allow us to replace existing variables.
// Whilst configuring we can replace variables in the cloned collection.
// After creation it is set back to not allow the addition of duplicate
// variable names.
var replacer2 = replacer.CloneAndConfigure(c => c
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

## Using the a custom dictionary cloner

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

// We configure the second replacer to allow us to replace existing variables.
// Whilst configuring we can replace variables in the cloned collection.
// After creation it is set back to not allow the addition of duplicate
// variable names.
var replacer2 = replacer.CloneAndConfigure(c => c
    .AddVariable("FirstName", "Other"),

    // THis custom cloner just returns the same dictionary
    // which resulsts in any variable changes being replicated
    // in `replacer` and `replacer2`
    originalDictionary => originalDictionary);

var result2 = replacer2.ReplaceVariables(
    "Hello $(FullName). Can I call you $(FirstName)?"
);
// result2 should be "Hello Other Jones. Can I call you Other?"

var firstName = replacer.Variables["FirstName"];
// firstName should be "Other" as the 
// we have shared the dictionary

var firstName2 = replacer2.Variables["FirstName"];
// firstName2 should be "Other"
```
