# Value Formatter

The default value formatter will return the `ToString` value of the variable. A `null` value will
return an empty string.

## Basic Example

Configuring a custom behaviour to format the variable value can be provided as shown below:

!!! note
    The value formatter can be used to format different types of object
    dependent on your needs.
    
```csharp { data-fiddle="qEOrwv" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariable("name", "Als")
    .WithValueFormatter(o => $"!{o}!"));

var result = replacer.ReplaceVariables("Hello $(name)");

// result will be "Hello !Als!"
```

## Per Type Formatting

The following example will format a date in a `dd/MM/YYY` format. Any other data type
will be formatted with `ToString`

```csharp { data-fiddle="eGJRX4" }
var now = new DateTime(2010, 1, 1);
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariable("name", "Als").AddVariable("today", now)
        .WithValueFormatter(o => o switch
        {
            DateTime v => v.ToString("dd/MM/yyyy"),
            _ => $"{o}"
        }));

var result = replacer.ReplaceVariables("Hello $(name). Today is $(today)");
// result will be "Hello Als. Today is 01/01/2010"
```

