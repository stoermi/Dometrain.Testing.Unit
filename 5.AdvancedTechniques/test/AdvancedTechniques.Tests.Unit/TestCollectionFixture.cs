using System;
using Xunit;

namespace AdvancedTechniques.Tests.Unit;

[CollectionDefinition("My Awesome Collection Fixture")]
public class TestCollectionFixture : ICollectionFixture<MyClassFixture>
{

}
