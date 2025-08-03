using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace AdvancedTechniques.Tests.Unit;

public class CalculatorTests
{
    private readonly Calculator _sut = new();

    [Theory]
    // [InlineData(5, 5, 10)]
    // [InlineData(-5, 5, 0)]
    // [InlineData(-15, -5, -20)]
    [MemberData(nameof(AddTestData))]
    public void Add_ShouldAddTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var result = _sut.Add(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    // [InlineData(5, 5, 0)]
    // [InlineData(15, 5, 10)]
    // [InlineData(-5, -5, 0)]
    // [InlineData(-15, -5, -10)]
    // [InlineData(5, 10, -5)]
    [ClassData(typeof(CalculatorSubtractTestData))]
    public void Subtract_ShouldSubtractTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var result = _sut.Subtract(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 5, 25)]
    [InlineData(50, 0, 0)]
    [InlineData(-5, 5, -25)]
    public void Multiply_ShouldMultiplyTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var result = _sut.Multiply(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 5, 1)]
    [InlineData(15, 5, 3)]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreIntegers(int a, int b, int expected)
    {
        // Act
        var result = _sut.Divide(a, b);

        // Assert
        result.Should().Be(expected);
    }

    public static IEnumerable<object[]> AddTestData()
    {
        yield return new object[] { 5, 5, 10 };
        yield return new object[] { -5, 5, 0 };
        yield return new object[] { -15, -5, -20 };
    }
}

public class CalculatorSubtractTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 5, 5, 0 };
        yield return new object[] { 15, 5, 10 };
        yield return new object[] { -5, -5, 0 };
        yield return new object[] { -15, -5, -10 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}