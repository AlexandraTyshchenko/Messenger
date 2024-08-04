using Messenger.Business.Commands;
using Messenger.Business.Interfaces;
using Messenger.Business.Options;
using Messenger.Infrastructure;
using Messenger.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Moq;
namespace Messenger.Business.Tests;

[TestFixture]
public class RefreshTokenCommandTests
{
    private Mock<UserManager<User>> _userManagerMock;
    private JwtSettings _jwtSettings;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ITokenService> _tokenServiceMock;
    private RefreshTokenCommandHandler _handler;
    [SetUp]
    public void SetUp()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null, null, null, null, null, null, null, null
        );

        _jwtSettings = new JwtSettings
        {
            Secret = "YourSecretKey",
            ValidIssuer = "YourIssuer",
            ValidAudience = "YourAudience",
            RefreshExpirationTimeInMinutes = 60
        };

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new RefreshTokenCommandHandler(
            _userManagerMock.Object,
             Microsoft.Extensions.Options.Options.Create(_jwtSettings),
            _unitOfWorkMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenRefreshTokenIsInvalidOrExpired()
    {
        // Arrange
        var command = new RefreshTokenCommand { RefreshToken = "invalidToken" };
        _unitOfWorkMock.Setup(u => u.RefreshTokens.GetRefreshTokenAsync(command.RefreshToken))
            .ReturnsAsync((RefreshToken)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.AreEqual("Invalid or expired refresh token.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotFound()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Token = "validToken",
            ExpiryDate = DateTime.Now.AddMinutes(5),
            User = new User { Id = Guid.NewGuid() }
        };
        var command = new RefreshTokenCommand { RefreshToken = refreshToken.Token };

        _unitOfWorkMock.Setup(u => u.RefreshTokens.GetRefreshTokenAsync(command.RefreshToken))
            .ReturnsAsync(refreshToken);
        _userManagerMock.Setup(u => u.FindByIdAsync(refreshToken.User.Id.ToString()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.AreEqual("Invalid refresh token.", result.ErrorMessage);
    }

    [Test]
    public async Task Handle_ShouldReturnNewToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Token = "validToken",
            ExpiryDate = DateTime.Now.AddMinutes(5),
            User = new User { Id = Guid.NewGuid() }
        };
        var command = new RefreshTokenCommand { RefreshToken = refreshToken.Token };

        _unitOfWorkMock.Setup(u => u.RefreshTokens.GetRefreshTokenAsync(command.RefreshToken))
            .ReturnsAsync(refreshToken);
        _userManagerMock.Setup(u => u.FindByIdAsync(refreshToken.User.Id.ToString()))
            .ReturnsAsync(refreshToken.User);
        _tokenServiceMock.Setup(t => t.CreateTokenAsync(refreshToken.User))
            .ReturnsAsync("newJwtToken");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("newRefreshToken");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.AreEqual("newJwtToken", result.Payload.Token);
        Assert.AreEqual("newRefreshToken", result.Payload.RefreshToken);
    }
}





