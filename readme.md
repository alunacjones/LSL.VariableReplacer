[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-variablereplacer.svg)](https://ci.appveyor.com/project/alunacjones/lsl-variablereplacer)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.VariableReplacer)](https://coveralls.io/github/alunacjones/LSL.VariableReplacer)
[![NuGet](https://img.shields.io/nuget/v/LSL.VariableReplacer.svg)](https://www.nuget.org/packages/LSL.VariableReplacer/)

# LSL.VariableReplacer

A library to provide variable replacement functionality. A source string can be taken by a configured variable replacer
and a prcoessed string will be returned by the use of the configured variable replacer.

Each variable value can reference other variables and a cyclic reference check will be performed upon execution
of the `ReplaceVariables` method of a created `IVariableReplacer`

## Quick Start

Using the default settings a variable replacer can be created with the following code:

```csharp
var replacer = new VariableReplacerFactory()
    .Build(c => c.AddVariable("name", "Als"));

var result = replacer.ReplaceVariables("Hi $(name)");

// result will have the value "Hi Als"
```

This default settings expect a variable placeholder should be of the format `$(VariableName)`

## Futher Documentation

More in-depth documentation can be found [here]()