---
tags:
    - variables
---
# Using Dictionaries

A convenience method `AddVariables` can be
used to quickly add a dictionary of variables as shown below:

!!! note

    The default value formatter will `ToString` all object values.
    A `null` value results in an empty string being substituted.
    See [here](./customisation/valueFormatter.md)  for customisation of this behaviour.

```csharp { data-fiddle="644uxQ" }
var variableDictionary = new Dictionary<string, object>
{
    ["name"] = "Als",
    ["age"] = 12
};

var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariables(variableDictionary));

var result = replacer.ReplaceVariables("Name = $(name), Age = $(age)");

// result will be "Name = Als, Age = 12"
```