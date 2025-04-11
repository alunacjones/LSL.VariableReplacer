---
tags:
    - variables
---
# Using Environment Variables

## Simple Configuration

You can add environment variables to your variable collection as follows:

!!! note
    The optional first parameter allows for filtering of environment variables.
    In this case we are only matching the environment variable that we set up.

    The optional prefix used in this example is `ENV_`. This is the default prefix
    and the parameter could have been omitted in this instance

!!! warning
    If the `ITransformer` that is used for the variable replacer
    cannot handle an environment variable's name then the environment
    variable will be ignored. This behaviour can be overriden as seen 
    [here](#advanced-configuration)

```csharp { data-fiddle="nRC1gP" }
Environment.SetEnvironmentVariable("NAME", "Als");

var replacer = new VariableReplacerFactory()
    .Build(c => c
        .AddEnvironmentVariables(key => key == "NAME", prefix: "ENV_"));

var result = replacer.ReplaceVariables("Hi $(ENV_NAME)");

// result will be "Hi Als"
```

## Advanced Configuration

An overload of the `AddEnvironmentVariables` method allows for further control
of how we select environment variables.

The following example shows us disabling the automatic filtering of environment
variables if their name is not valid for the `ITransformer` in use.

!!! note
    There may be scenarios whereby this is desirable in order to check what variables
    are being added and which ones are problematic via the exception that is thrown
    at the point of an `IVariableReplacer` being created.

```csharp { data-fiddle="FPIxY9" }
// This environment variable's key has a '-' in it 
// and the default transformer does not allow this.
Environment.SetEnvironmentVariable("ALS-NAME", "Als");

// This will throw an InvalidVariableNamesException with a message 
// that will show the problematic environment variables
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .AddEnvironmentVariables(c => c.DisableInvalidVariableNameFilter()));
```