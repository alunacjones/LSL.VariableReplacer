---
tags:
    - variables
---
# Using Environment Variables

You can add environment variables to your variable collection as follows:

!!! note
    The optional first parameter allows for filtering of environment variables.
    In this case we are only matching the environment variable that we set up.

    The optional prefix used in this example is `ENV_`. This is the default prefix
    and the parameter could have been omitted in this instance

!!! warning
    If the `ITransformer` that is used for the variable replacer
    cannot handle an environment variable's name then the environment
    variable will be ignored

```csharp { data-fiddle="nRC1gP" }
Environment.SetEnvironmentVariable("NAME", "Als");

var replacer = new VariableReplacerFactory()
    .Build(c => c
        .AddEnvironmentVariables(key => key == "NAME", prefix: "ENV_"));

var result = replacer.ReplaceVariables("Hi $(ENV_NAME)");

// result will be "Hi Als"
```