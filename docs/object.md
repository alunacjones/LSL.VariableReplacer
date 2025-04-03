# Adding Variables From An Object

You can also add an objects properties to a dictionary.

!!! note
    Nested objects will be traversed and a composite key is generated based
    on the path to the property

```csharp { data-fiddle="SBz54u" }
var sut = new VariableReplacerFactory()
    .Build(c => c.AddVariablesFromObject(new { 
        name = "Als", 
        age = 12, 
        other = new { 
            codes = true
        } 
    }));

var result = sut.ReplaceVariables("Hello $(name). $(other.codes)");
// result will be "Hello Als. True"
```