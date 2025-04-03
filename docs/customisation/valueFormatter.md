# Value Formatter

The default value formatter will return the `ToString` value of the variable. A `null` value will
return an empty string.

Configuring a custom behaviour e.g. to format certain types can be provided as shown below:

```csharp { data-fiddle="qEOrwv" }
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariable("name", "Als")
    .WithValueFormatter(o => $"!{o}!"));

var result = replacer.ReplaceVariables("Hello $(name)");

// result will be "Hello !Als!"
```