using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public void VariableReplacerFactory_GivenABuildWithACustomTransformer_ItShouldReplaceAnyVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c
                .WithTransformer(new NotVeryUsefulTransformer())
                .AddVariable("name", "Als"));

        sut.ReplaceVariables("Hi $name$. Where doth $name$ hail from?")
            .Should()
            .Be("Hi Als. Where doth Als hail from?");
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
    public void VariableReplacerFactory_GivenABuildThatHasACustomMessageWhenNotFound_ItProduceTheExpectedResult()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c.WhenVariableNotFound(variableName => $"ERROR:{variableName}"));

        sut.ReplaceVariables("Hello $(Unknown)")
            .Should()
            .Be("Hello ERROR:Unknown");
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

    [Test]
    public void VariableReplacerFactory_GivenABuildWithCustomRegexOptions_ItShouldReplaceAllVariables()
    {
        var sut = new VariableReplacerFactory()
            .Build(c => c
                .WithDefaultTransformer(regexOptions: RegexOptions.Compiled)
                .AddVariables(new Dictionary<string, object>
                {
                    ["FirstName"] = "Al",
                    ["LastName"] = "Jones"
                }));

        sut.ReplaceVariables("Hello $(FirstName) $(LastName).\n\r\n Can I call you $(FirstName)?")
            .Should()
            .Be("Hello Al Jones.\n\r\n Can I call you Al?");         
    }    

    [Test]
    public void VariableReplacerFactory_GivenABuildWithCustomOptions_ItShouldReplaceAllVariables()
    {
        var replacer = new VariableReplacerFactory()
            .Build(c => c
                .WithDefaultTransformer("[", "]", (command, value) => value, RegexOptions.None, TimeSpan.FromSeconds(1))
                .AddVariables(new Dictionary<string, object>
                {
                    ["FirstName"] = "Al",
                    ["LastName"] = "Jones"
                }));

        // our replacer will timeout after 1 second now
        // our replacer's regular expression will not be compiled
        var result = replacer.ReplaceVariables(
            "Hello [FirstName] [LastName]. Can I call you [FirstName]?"
        );
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
        Environment.SetEnvironmentVariable("NAME", "Als");
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

public class NotVeryUsefulTransformer : ITransformer
{
    public string Transform(IVariableResolutionContext variableResolutionContext)
    {
        // Get the source string from the context
        var sourceString = variableResolutionContext.Source;

        // we will use this to build the replaced string
        var result = new StringBuilder();

        var index = 0;

        // Loop through each character in the string
        while (index < sourceString.Length)
        {            
            var currentCharacter = sourceString[index];

            // Check to see if we have found the start of a variable
            if (currentCharacter == '$')
            {
                // we build the name of the varaible with this
                var name = new StringBuilder();

                // Move beyond the $ start character
                index++;

                // Collect all the variable name characters
                // until we hit the closing $
                while (sourceString[index] != '$')
                {
                    name.Append(sourceString[index]);
                    index++;
                }                

                // resolve the variable value
                var variableValue = variableResolutionContext.VariableResolver.Resolve(name.ToString());

                // Append the value to our result StringBuilder
                result.Append(variableValue);
                index++;

                // Continue processing the string
                continue;
            }

            // Not at the start of a variable so just append the character
            result.Append(currentCharacter);

            // Move to the next character
            index++;       
        }

        // Return the variable-replaced result
        return result.ToString();
    }
}