using AutoMapper;
using Messenger.Business.Commands;
using Messenger.Business.Dtos;
using Messenger.Business.Interfaces;
using Messenger.Business.Profiles;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Net;

namespace Messenger.Business.Tests;

public class AuthenticateUserCommandTests
{
    private IMapper _mapper;
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<ITokenService> _tokenServiceMock;
    private AuthenticateUserCommandHandler _handler;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null, null, null, null, null, null, null, null);
    }

    [SetUp]
    public void SetUp()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new AuthenticateUserCommandHandler(
            _userManagerMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            UserLogin = new UserLoginDto
            {
                UserName = "testuser",
                Password = "password123"
            }
        };

        var user = new User
        {
            UserName = command.UserLogin.UserName
        };

        var token = "generated-token";
        var refreshToken = "generated-refresh-token";
        var tokenDto = new TokenDto
        {
            Token = token,
            RefreshToken = refreshToken
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(command.UserLogin.UserName))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, command.UserLogin.Password))
            .ReturnsAsync(true);

        _userManagerMock.Setup(um => um.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _tokenServiceMock.Setup(ts => ts.CreateTokenAsync(user))
            .ReturnsAsync(token);

        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken())
            .Returns(refreshToken);

        _tokenServiceMock.Setup(ts => ts.StoreRefreshTokenAsync(user, refreshToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(token, result.Payload.Token);
        Assert.AreEqual(refreshToken, result.Payload.RefreshToken);
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenEmailIsNotConfirmed()
    {
        // Arrange
        var user = new User { UserName = "testuser" };
        var command = new AuthenticateUserCommand
        {
            UserLogin = new UserLoginDto { UserName = "testuser", Password = "password" }
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(command.UserLogin.UserName))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, command.UserLogin.Password))
            .ReturnsAsync(true);
        _userManagerMock.Setup(um => um.IsEmailConfirmedAsync(user))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.AreEqual("Email not confirmed.", result.ErrorMessage);

        // Verify that no further methods were called
        _tokenServiceMock.Verify(ts => ts.CreateTokenAsync(It.IsAny<User>()), Times.Never);
        _tokenServiceMock.Verify(ts => ts.GenerateRefreshToken(), Times.Never);
        _tokenServiceMock.Verify(ts => ts.StoreRefreshTokenAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenCredentialsAreInvalid()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            UserLogin = new UserLoginDto
            {
                UserName = "testuser",
                Password = "wrongpassword"
            }
        };

        var user = new User
        {
            UserName = command.UserLogin.UserName
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(command.UserLogin.UserName))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, command.UserLogin.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.AreEqual("Invalid credentials.", result.ErrorMessage);
    }


}
