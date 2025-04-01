using System;
using System.Collections.Generic;
using System.Linq;
using DotNetEnv;
using FluentAssertions;

namespace LSL.VariableReplacer.Tests;

public class VariableReplacerFactoryTests
{
    [Test]
    public void VariableReplacerFactory_GivenABuildWithNoVariables_ItShouldReplaceAnyVariablesWithNotFound()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => {});

        sut.ReplaceVariables("Hello $(Unknown)")
            .Should()
            .Be("Hello NOTFOUND:Unknown");         
    }

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariablesAndACustomerFormatter_ItShouldReplaceAnyVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariable("name", "Als").WithValueFormatter(o => $"!{o}!"));

        sut.ReplaceVariables("Hello $(name)")
            .Should()
            .Be("Hello !Als!");         
    }    

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariablesFromAnObject_ItShouldReplaceAnyVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariablesFromObject(new { name = "Als", age = 12, other = new { codes = true} }));

        sut.Variables.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            ["name"] = "Als",
            ["age"] = 12,
            ["other.codes"] = true
        });

        sut.ReplaceVariables("Hello $(name). $(other.codes)")
            .Should()
            .Be("Hello Als. True");         
    }        

    [Test]
    public void VariableReplacerFactory_GivenTheDefaultAddBehaviour_ItShouldThrowAnExceptionWhenAddingTheSameVariable()
    {
        new Action(() => new VariableReplacerFactory()
            .Build(c => c.AddVariable("same", 1).AddVariable("same", 2)))
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("An item with the same key has already been added. Key: same");
    }

    [Test]
    public void VariableReplacerFactory_GivenTheReplaceBehaviour_ItShouldHaveTheLastOneAdded()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.WithReplaceVariableBehaviour().AddVariable("same", 1).AddVariable("same", 2));

        sut.Variables.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            ["same"] = 2
        });
    }

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariables_ItShouldReplaceAllVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariables(new Dictionary<string, object>
            {
                ["FirstName"] = "Al",
                ["LastName"] = "Jones"
            }));

        sut.ReplaceVariables("Hello $(FirstName) $(LastName). Can I call you $(FirstName)?")
            .Should()
            .Be("Hello Al Jones. Can I call you Al?");         
    }

    [Test]
    public void VariableReplacerFactory_GivenABuildWithRecursiveVariables_ItShouldReplaceAllVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariables(new Dictionary<string, object>
            {
                ["FirstName"] = "Al",
                ["LastName"] = "Jones",
                ["FullName"] = "$(FirstName) $(LastName)"
            }));

        sut.ReplaceVariables("Hello $(FullName). Can I call you $(FirstName)?")
            .Should()
            .Be("Hello Al Jones. Can I call you Al?");         
    }

    [Test]
    public void VariableReplacerFactory_WhenWeClone_ThenTheNewReplacerShouldNotAffectTheOriginal()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariables(new Dictionary<string, object>
            {
                ["FirstName"] = "Al",
                ["LastName"] = "Jones",
                ["FullName"] = "$(FirstName) $(LastName)"
            }));

        sut.ReplaceVariables("Hello $(FullName). Can I call you $(FirstName)?")
            .Should()
            .Be("Hello Al Jones. Can I call you Al?");

        var sut2 = sut.CloneAndConfigure(c => c.WithReplaceVariableBehaviour()
            .AddVariable("FirstName", "Other"));

        sut2.ReplaceVariables("Hello $(FullName). Can I call you $(FirstName)?")
            .Should()
            .Be("Hello Other Jones. Can I call you Other?");

        sut.Variables["FirstName"].Should().Be("Al");
        sut2.Variables["FirstName"].Should().Be("Other");
    }    

    [Test]
    public void VariableReplacerFactory_GivenABuildThatThrowsWhenAVariableIsNotFound_ItShouldThrowTheExpectedException()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.ThrowIfVariableNotFound());

        new Action(() => sut.ReplaceVariables("Hello $(Unknown)"))
            .Should()
            .Throw<ArgumentException>().WithMessage("Variable 'Unknown' not found");    
    }        

    [Test]
    public void VariableReplacerFactory_GivenABuildWithCyclicRecursiveVariables_ItShouldTherowTheExpectedException()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.AddVariables(new Dictionary<string, object>
            {
                ["FirstName"] = "Al",
                ["LastName"] = "Jones",
                ["FullName"] = "$(FirstName) $(LastName) $(Other)",                
                ["Other"] = "Stuff $(Another)",
                ["Another"] = "$(FullName)"
            }));

        new Action(() => sut.ReplaceVariables("Hello $(FullName). Can I call you $(FirstName)?"))
            .Should()
            .Throw<ArgumentException>().WithMessage("Cyclic dependency detected on path: FullName -> Other -> Another -> FullName");
    }        

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariablesAndCustomVariableDelimeters_ItShouldReplaceAllVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c
                .WithDefaultTransformer("$${", "}")
                .AddVariables(new Dictionary<string, object>
                {
                    ["FirstName"] = "Al",
                    ["LastName"] = "Jones"
                }));

        sut.ReplaceVariables("Hello $${FirstName} $${LastName}. Can I call you $${FirstName}?")
            .Should()
            .Be("Hello Al Jones. Can I call you Al?");         
    }

    [TestCase("a/path")]
    [TestCase("a/path/")]
    public void VariableReplacerFactory_GivenABuildWithVariablesAndCustomVariableDelimetersAndKeyPreProcessor_ItShouldReplaceAllVariables(string path)
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c
                .WithDefaultTransformer(commandProcessor: (command, value) => command switch
                    {
                        "trim" => value.Trim(),
                        "ensureSlash" => value.EndsWith("/") ? value : $"{value}/",
                        _ => value
                    }
                )
                .AddVariables(new Dictionary<string, object>
                {
                    ["FirstName"] = "   Al    ",
                    ["LastName"] = "   Jones   ",
                    ["Path"] = path,
                    ["Other"] = null,
                    ["FullName"] = "$(FirstName:trim) $(LastName:trim)"
                }));

        sut.ReplaceVariables("Hello $(FullName). You normalised '$(Path)' to '$(Path:ensureSlash)'$(Other:trim)")
            .Should()
            .Be($"Hello Al Jones. You normalised '{path}' to 'a/path/'");
    }    

    [Test]
    public void VariableReplacerFactory_GivenABuildWithEnvironmentVariables_ItShouldReplaceAllVariables()
    {
        Env.LoadContents(
            """
            ALS_NAME=Als
            """
        );
        
        var prefix = "ALS_";

        var sut = new VariableReplacerFactory()
            .Build(c => c
                .AddEnvironmentVariables(key => key.StartsWith(prefix), prefix: null));

        sut.Variables.Should().BeEquivalentTo(new Dictionary<string, object>
        {
            ["ALS_NAME"] = "Als"
        });

        sut.ReplaceVariables("Hi $(ALS_NAME)").Should().Be("Hi Als");
    }

    [Test]
    public void VariableReplacerFactory_GivenABuildWithEnvironmentVariablesAndNoFilter_ItShouldContainTheExpectedVariables()
    {
        Env.LoadContents(
            """
            ALS_NAME=Als
            """
        );

        var sut = new VariableReplacerFactory()
            .Build(c => c
                .AddEnvironmentVariables(prefix: null));

        sut.Variables.Count.Should().BeGreaterThan(1);
    }    

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariablesButTheDictionaryIsNull_ItShouldThrowTheExpectedException()
    {
        new Action(() =>  new VariableReplacerFactory()
            .Build(c => c.AddVariables(null))
        ).Should().Throw<ArgumentNullException>().WithMessage("Argument cannot be null (Parameter 'variableDictionary')");
    }

    [Test]
    public void VariableReplacerFactory_GivenABuildWithVariablesButWeAddAVariableWithANullKey_ItShouldThrowTheExpectedException()
    {
        new Action(() =>  new VariableReplacerFactory()
            .Build(c => c.AddVariables(new Dictionary<string, object>
            {
                ["Name"] = "Als",
            }).AddVariable(null, null))
        ).Should().Throw<ArgumentNullException>().WithMessage("Argument cannot be null (Parameter 'name')");
    }    
}