using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;
using Xunit;

namespace Users.Api.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly UserService _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ILoggerAdapter<UserService> _logger = Substitute.For<ILoggerAdapter<UserService>>();

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
            var users = await _sut.GetAllAsync();

            // Assert
            users.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers_WhenSomeUsersExist()
        {
            // Arrange
            var expectedUsers = new User[]
            {
                new User ()
                {
                     Id = Guid.NewGuid(),
                     FullName = "Test",
                },
                new User ()
                {
                     Id = Guid.NewGuid(),
                     FullName = "Test2",
                }
            };
            _userRepository.GetAllAsync().Returns(expectedUsers);

            // Act
            var users = await _sut.GetAllAsync();

            // Assert
            users.Should().BeEquivalentTo(expectedUsers);
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
        public async Task GetAllAsync_ShouldThrowAndLogException_WhenRepoThrowsException()
        {
            // Arrange
            Exception exc = new Exception("TestException");
            _userRepository.GetAllAsync().Throws(exc);

            // Act
            Func<Task> result = async () => await _sut.GetAllAsync();

            // Assert
            await result.Should().ThrowAsync<Exception>().WithMessage("TestException");
            _logger.Received(1).LogError(Arg.Is(exc), Arg.Is("Something went wrong while retrieving all users"));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var expectedUser = new User()
            {
                Id = Guid.NewGuid(),
                FullName = "Test"
            };
            _userRepository.GetByIdAsync(expectedUser.Id).Returns(expectedUser);

            // Act
            var user = await _sut.GetByIdAsync(expectedUser.Id);

            // Assert
            user.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var user = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogMessages_WhenInvoked()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _userRepository.GetByIdAsync(id).ReturnsNull();

            // Act
            await _sut.GetByIdAsync(id);

            // Assert
            _logger.Received(1).LogInformation(Arg.Is("Retrieving user with id: {0}"), Arg.Is(id));
            _logger.Received(1).LogInformation(Arg.Is("User with id {0} retrieved in {1}ms"), Arg.Is(id), Arg.Any<long>());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogMessageAndThrowsException_WhenRepoThrowsException()
        {
            // Arrange
            var exc = new Exception("TestMessage");
            var id = Guid.NewGuid();
            _userRepository.GetByIdAsync(id).Throws(exc);

            // Act
            var action = async () => await _sut.GetByIdAsync(id);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("TestMessage");
            _logger.Received(1).LogError(Arg.Is(exc), Arg.Is("Something went wrong while retrieving user with id {0}"), Arg.Is(id));
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTrue_WhenValidUserSubmitted()
        {
            // Arrange
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FullName = "Test"
            };
            _userRepository.CreateAsync(user).Returns(true);

            // Act
            var result = await _sut.CreateAsync(user);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessages_WhenInvoked()
        {
            // Arrange
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FullName = "Test"
            };

            // Act
            var result = await _sut.CreateAsync(user);

            // Assert
            _logger.Received(1).LogInformation(Arg.Is("Creating user with id {0} and name: {1}"), Arg.Is(user.Id), Arg.Is(user.FullName));
            _logger.Received(1).LogInformation(Arg.Is("User with id {0} created in {1}ms"), Arg.Is(user.Id), Arg.Any<long>());
        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessageAndThrowsException_WhenRepoThrowsException()
        {
            // Arrange
            var exc = new Exception("TestMessage");
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FullName = "Test"
            };
            _userRepository.CreateAsync(user).Throws(exc);

            // Act
            var action = async () => await _sut.CreateAsync(user);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("TestMessage");
            _logger.Received(1).LogError(Arg.Is(exc), Arg.Is("Something went wrong while creating a user"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenExistingUserIdSubmitted()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _userRepository.DeleteByIdAsync(id).Returns(true);

            // Act
            var result = await _sut.DeleteByIdAsync(id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNonExistingUserIdSubmitted()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _userRepository.DeleteByIdAsync(id).Returns(false);

            // Act
            var result = await _sut.DeleteByIdAsync(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogMessages_WhenInvoked()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            // Act
            var result = await _sut.DeleteByIdAsync(id);

            // Assert
            _logger.Received(1).LogInformation(Arg.Is("Deleting user with id: {0}"), Arg.Is(id));
            _logger.Received(1).LogInformation(Arg.Is("User with id {0} deleted in {1}ms"), Arg.Is(id), Arg.Any<long>());
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogMessageAndThrowsException_WhenRepoThrowsException()
        {
            // Arrange
            var exc = new Exception("TestMessage");
            Guid id = Guid.NewGuid();
            _userRepository.DeleteByIdAsync(id).Throws(exc);

            // Act
            var action = async () => await _sut.DeleteByIdAsync(id);

            // Assert
            await action.Should().ThrowAsync<Exception>().WithMessage("TestMessage");
            _logger.Received(1).LogError(Arg.Is(exc), Arg.Is("Something went wrong while deleting user with id {0}"), Arg.Is(id));
        }
    }
}
