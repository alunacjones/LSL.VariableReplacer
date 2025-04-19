---
tags:
    - variables
---
# Adding Variables From An Object

You can also add an object's properties to the variable collection using the 
`AddVariablesFromObject` method on the configuration object.

## With No Customisation

!!! note
    Nested objects will be traversed and a composite key is generated based
    on the path to the property

```csharp { data-fiddle="SBz54u" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariablesFromObject(new { 
        name = "Als", 
        age = 12, 
        other = new { 
            codes = true
        } 
    }));

var result = replacer.ReplaceVariables("Hello $(name). $(other.codes)");
// result will be "Hello Als. True"
```

## With a Custom Configuration

If you use the optional `configurator` parameter then you can specify
extra customisation as to how the object is queried and the variables are created.

### With a Custom Variable Name Prefix

You can provide a string that will prefix all the generated variable names if you need
to further namespace the generated variable names.

```csharp { data-fiddle="aYlge7" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariablesFromObject(new
    {
        name = "Als",
        age = 12,
        other = new
        {
            codes = true
        }
    },
    c => c.WithPrefix("MyObj.")
));

var result = replacer.ReplaceVariables(
    "Hello $(MyObj.name). $(MyObj.other.codes)"
);
// result will be "Hello Als. True"
```
### With a Custom Property Path Separator

You can provide a string to use as an alternative to the default property path separator of `.`
as follows:

!!! warning
    The value of the separator can cause the build of an `IVariableReplacer` to fail
    if the `ITransformer` that is utilised cannot handle a variable name that is used.

    The default transformer will throw an exception if a variable is added with a name
    that collides with its settings and provide information about how to fix the issue.

```csharp { data-fiddle="Ii1MBR" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariablesFromObject(new { 
        name = "Als", 
        age = 12, 
        other = new { 
            codes = true
        }
    },
    c => c.WithPropertyPathSeparator("_")));

var result = replacer.ReplaceVariables("Hello $(name). $(other_codes)");

// result will be "Hello Als. True"
```

### With a Property Filter

A filter can be passed in to allow you to filter properties from the passed in object.
The following example shows how this can be performed on a property name and the
object traversal path:

```csharp { data-fiddle="rBvd9J" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariablesFromObject(new { 
        name = "Als", 
        age = 12, 
        other = new { 
            codes = true
        },
        never = new {
            ommitted = true
        } 
    },
    c => c.WithPropertyFilter(
        p => p.Property.Name != string.Empty && p.ParentPath != "never"
    )
));

var result = replacer.ReplaceVariables(
    "Hello $(name). $(other.codes) $(never_omitted)"
);

// result will be "Hello Als. True NOTFOUND:never_omitted");
```

### Advanced Scenarios

If you require more advanced scenarios for inspecting an object then it is probably best
to use a third party library for querying an object and extracting data.