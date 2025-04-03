# The Default Transformer

The default transformer uses a regular expression to match the variables
in a source string. This can be customised using the `WithDefaultTransformer` method
on the configuration object.

## Custom Variable Delimeters

The default format searches for variables of the format `$(VariableName)`. This can be customised as
follows: 

```csharp { data-fiddle="Hj5Ng8" }
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithDefaultTransformer("$${", "}")
        .AddVariables(new Dictionary<string, object>
        {
            ["FirstName"] = "Al",
            ["LastName"] = "Jones"
        }));

var result = replacer.ReplaceVariables(
    "Hello $${FirstName} $${LastName}. Can I call you $${FirstName}?"
);

// result will be "Hello Al Jones. Can I call you Al?"

```

## Custom Regex Options

The default `RegexOptions` are setup as `RegexOptions.Compiled`.

This can be customised as follows:

```csharp { data-fiddle="RJutMU" }
// The replacer's regular expression will now not be compiled
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithDefaultTransformer(regexOptions: RegexOptions.None)
        .AddVariables(new Dictionary<string, object>
        {
            ["FirstName"] = "Al",
            ["LastName"] = "Jones"
        }));
```

## Custom Regex Timeout

The default `Regex` timeout is set to 10 seconds. This can be modified as follows:

```csharp { data-fiddle="ltvnV6" }
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithDefaultTransformer(regexTimeOut: TimeSpan.FromSeconds(1))
        .AddVariables(new Dictionary<string, object>
        {
            ["FirstName"] = "Al",
            ["LastName"] = "Jones"
        }));

// our replacer will timeout after 1 second now
var result = replacer.ReplaceVariables(
    "Hello $(FirstName) $(LastName). Can I call you $(FirstName)?"
);

```

## Custom Commands

The default transformer allows for extra modification of a formatted value that can enable
extension functionality like trimming a string.

The following example adds the functionality for a trim command:

!!! note
    We are using variables within other variables here too.

!!! warning    
    An exception is thrown if a cyclic dependency is detected.
```csharp  { data-fiddle="f5nv9v" }
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithDefaultTransformer(commandProcessor: (command, value) => 
            command switch
            {
                "trim" => value.Trim(),
                _ => string.IsNullOrEmpty(command) 
                    ? value
                    : throw new ArgumentException($"Unknown command '{command}'")
            }
        )
        .AddVariables(new Dictionary<string, object>
        {
            // The spaces in these values will
            // be trimmed for "FullName"
            ["FirstName"] = "   Al    ",
            ["LastName"] = "   Jones   ",
            ["FullName"] = "$(FirstName:trim) $(LastName:trim)"
        }));

var result = replacer.ReplaceVariables("Hello $(FullName)");
// result will be "Hello Al Jones"
```

## Using All the Options

Since all the parameters are optional, any combination can be
used to configure the default transformer.

The following example sets up all options:

```csharp { data-fiddle="cdOvVw" }
var replacer = new VariableReplacerFactory()
    .Build(c => c
        .WithDefaultTransformer(
            "[", 
            "]", 
            (command, value) => value, 
            RegexOptions.None, 
            TimeSpan.FromSeconds(1))
        .AddVariables(new Dictionary<string, object>
        {
            ["FirstName"] = "Al",
            ["LastName"] = "Jones"
        }));

// our replacer will timeout after 1 second now
// our replacer's regular expression will not be compiled
// any command will be ignored (i.e. the value will just be returned)
var result = replacer.ReplaceVariables(
    "Hello [FirstName] [LastName]. Can I call you [FirstName]?"
);
```