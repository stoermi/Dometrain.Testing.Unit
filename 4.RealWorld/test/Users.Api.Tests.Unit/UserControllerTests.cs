using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Users.Api.Contracts;
using Users.Api.Controllers;
using Users.Api.Mappers;
using Users.Api.Models;
using Users.Api.Services;
using Xunit;

namespace Users.Api.Tests.Unit;

public class UserControllerTests
{
    private readonly UserController _sut;
    private readonly IUserService _userService = Substitute.For<IUserService>();

    public UserControllerTests()
    {
        _sut = new UserController(_userService);
    }

    [Fact]
    public async Task GetAll_ShouldReturnsOkWithUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), FullName = "John Doe" },
            new User { Id = Guid.NewGuid(), FullName = "Jane Doe" }
        };
        _userService.GetAllAsync().Returns(users);
        var usersResponse = users.Select(x => x.ToUserResponse());

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.As<IEnumerable<UserResponse>>().Should().BeEquivalentTo(usersResponse);
    }

    [Fact]
    public async Task GetAll_ShouldReturnsOkWithNoUsers_WhenNoUsersExist()
    {
        // Arrange
        _userService.GetAllAsync().Returns(Enumerable.Empty<User>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var usersResponse = okResult.Value.As<IEnumerable<UserResponse>>().Should().BeEmpty();
    }

    [Fact]
    public async Task GetByID_ShouldReturnsOkWithUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), FullName = "John Doe" };
        _userService.GetByIdAsync(user.Id).Returns(user);
        var userResponse = user.ToUserResponse();

        // Act
        var result = await _sut.GetById(user.Id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(userResponse);
    }

    [Fact]
    public async Task GetByID_ShouldReturnsNotFoundWithNoBody_WhenNoUserExists()
    {
        // Arrange
        _userService.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        // Act
        var result = await _sut.GetById(Guid.NewGuid());

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedWithUser_WhenValidUserDataSubmitted()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest { FullName = "John Doe" };
        User user = new User();
        _userService.CreateAsync(Arg.Do<User>(x => user = x)).Returns(true);

        // Act
        var result = await _sut.Create(createUserRequest);

        // Assert
        var userResponse = user.ToUserResponse();
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(userResponse);
        createdResult.RouteValues!["id"].Should().Be(userResponse.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenInvalidUserDataSubmitted()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest();
        _userService.CreateAsync(Arg.Any<User>()).Returns(false);

        // Act
        var result = await _sut.Create(createUserRequest);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DeleteById_ShouldReturnOk_WhenUserDeleted()
    {
        // Arrange
        _userService.DeleteByIdAsync(Arg.Any<Guid>()).Returns(true);

        // Act
        var result = await _sut.DeleteById(Guid.NewGuid());

        // Assert
        var okResult = result.Should().BeOfType<OkResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userService.DeleteByIdAsync(userId).Returns(false);

        // Act
        var result = await _sut.DeleteById(userId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }
}