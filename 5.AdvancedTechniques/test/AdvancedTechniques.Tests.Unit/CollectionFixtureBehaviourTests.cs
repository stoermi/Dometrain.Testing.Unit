using System;
using Xunit;
using Xunit.Abstractions;

namespace AdvancedTechniques.Tests.Unit;

[Collection("My Awesome Collection Fixture")]
public class CollectionFixtureBehaviourTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly MyClassFixture _fixture;

    public CollectionFixtureBehaviourTests(ITestOutputHelper testOutputHelper, MyClassFixture fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Fact]
    public void ExampleTest1()
    {
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
    }

    [Fact]
    public void ExampleTest2()
    {
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
    }
}

[Collection("My Awesome Collection Fixture")]
public class CollectionFixtureBehaviourTests2
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly MyClassFixture _fixture;

    public CollectionFixtureBehaviourTests2(ITestOutputHelper testOutputHelper, MyClassFixture fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Fact]
    public void ExampleTest1()
    {
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
    }

    [Fact]
    public void ExampleTest2()
    {
        _testOutputHelper.WriteLine($"The Guid was: {_fixture.Id}");
    }
}
