using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;
using Xunit;

namespace Users.Api.Tests.Unit.Application;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ILoggerAdapter<UserService> _logger = Substitute.For<ILoggerAdapter<UserService>>();
    private readonly User _userNickChapsas = new()
    {
        Id = Guid.NewGuid(),
        FullName = "Nick Chapsas"
    };

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenSomeUsersExist()
    {
        // Arrange
        var nickChapsas = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Nick Chapsas"
        };
        var expectedUsers = new[]
        {
            nickChapsas
        };
        _userRepository.GetAllAsync().Returns(expectedUsers);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        //result.Single().Should().BeEquivalentTo(nickChapsas);
        result.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
    {
        // Arrange
        _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>());

        // Act
        await _sut.GetAllAsync();

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Retrieving all users"));
        _logger.Received(1).LogInformation(Arg.Is("All users retrieved in {0}ms"), Arg.Any<long>());
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        // Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.GetAllAsync()
            .Throws(sqliteException);

        // Act
        var requestAction = async () => await _sut.GetAllAsync();

        // Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>().WithMessage("Something went wrong");
        _logger.Received(1).LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while retrieving all users"));
    }

    // GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        _userRepository.GetByIdAsync(_userNickChapsas.Id).Returns(_userNickChapsas);

        // Act
        var result = await _sut.GetByIdAsync(_userNickChapsas.Id);

        // Assert
        result.Should().BeEquivalentTo(_userNickChapsas);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository.GetByIdAsync(Arg.Is(_userNickChapsas.Id)).ReturnsNull();

        // Act
        var result = await _sut.GetByIdAsync(_userNickChapsas.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogMessages_WhenInvoked()
    {
        // Arrange
        _userRepository.GetByIdAsync(_userNickChapsas.Id).Returns(_userNickChapsas);

        // Act
        await _sut.GetByIdAsync(_userNickChapsas.Id);

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Retrieving user with id: {0}"), Arg.Is(_userNickChapsas.Id));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} retrieved in {1}ms"), Arg.Is(_userNickChapsas.Id), Arg.Any<long>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        // Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.GetByIdAsync(_userNickChapsas.Id)
            .Throws(sqliteException);

        // Act
        var requestAction = async () => await _sut.GetByIdAsync(_userNickChapsas.Id);

        // Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>().WithMessage("Something went wrong");
        _logger.Received(1).LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while retrieving user with id {0}"), Arg.Is(_userNickChapsas.Id));
    }
    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenUserIsCreated()
    {
        // Arrange
        _userRepository.CreateAsync(_userNickChapsas).Returns(true);

        // Act
        var result = await _sut.CreateAsync(_userNickChapsas);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldLogCorrectMessages_WhenCreatedSuccesfully()
    {
        // Arrange
        _userRepository.CreateAsync(_userNickChapsas).Returns(true);

        // Act
        await _sut.CreateAsync(_userNickChapsas);

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Creating user with id {0} and name: {1}"), Arg.Is(_userNickChapsas.Id), Arg.Is(_userNickChapsas.FullName));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} created in {1}ms"), Arg.Is(_userNickChapsas.Id), Arg.Any<long>());
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
    {
        // Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.CreateAsync(_userNickChapsas)
            .Throws(sqliteException);

        // Act
        var requestAction = async () => await _sut.CreateAsync(_userNickChapsas);

        // Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>().WithMessage("Something went wrong");
        _logger.Received(1).LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while creating a user"));
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        _userRepository.DeleteByIdAsync(_userNickChapsas.Id).Returns(true);

        // Act
        var result = await _sut.DeleteByIdAsync(_userNickChapsas.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldNotDeleteUser_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepository.DeleteByIdAsync(_userNickChapsas.Id).Returns(false);

        // Act
        var result = await _sut.DeleteByIdAsync(_userNickChapsas.Id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogCorrectMessages_WhenDeletedSuccesfully()
    {
        // Arrange
        _userRepository.DeleteByIdAsync(_userNickChapsas.Id).Returns(true);

        // Act
        await _sut.DeleteByIdAsync(_userNickChapsas.Id);

        // Assert
        _logger.Received(1).LogInformation(Arg.Is("Deleting user with id: {0}"), Arg.Is(_userNickChapsas.Id));
        _logger.Received(1).LogInformation(Arg.Is("User with id {0} deleted in {1}ms"), Arg.Is(_userNickChapsas.Id), Arg.Any<long>());
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessageAndException_WhenUserDoesNotExists()
    {
        // Arrange
        var sqliteException = new SqliteException("Something went wrong", 500);
        _userRepository.DeleteByIdAsync(_userNickChapsas.Id)
            .Throws(sqliteException);

        // Act
        var requestAction = async () => await _sut.DeleteByIdAsync(_userNickChapsas.Id);

        // Assert
        await requestAction.Should()
            .ThrowAsync<SqliteException>().WithMessage("Something went wrong");
        _logger.Received(1).LogError(Arg.Is(sqliteException), Arg.Is("Something went wrong while deleting user with id {0}"), Arg.Is(_userNickChapsas.Id));
    }
}
