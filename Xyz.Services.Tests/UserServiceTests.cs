using AutoFixture.Xunit2;
using Microsoft.Extensions.Logging;
using Xyz.Infrastructure.Abstractions;
using Xyz.Models;
using Xyz.SDK.Dao;
using Xyz.SDK.Service;
using Xyz.Services.Validators;

namespace Xyz.Services.Tests;
using System.Threading.Tasks;
using Moq;
using Xunit;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<ILogger<ServiceBase>> _mockLogger = new();
    
    // sut : System Under Tests
    private readonly UserService _sut;

    public UserServiceTests()
    {
        var userValidator = new UserValidator(_mockUserRepository.Object);
        _sut = new UserService(_mockUnitOfWork.Object, _mockLogger.Object, _mockUserRepository.Object, userValidator);
    }

    [Theory]
    [AutoData]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists(string emailPrefix)
    {
        // Arrange
        var email = $"{emailPrefix}@someDomain.xyz";
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync(new User { Email = email });

        // Act
        var result = await _sut.EmailExistsAsync(email);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(repo => repo.GetUserByEmailAsync(email), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist(string emailPrefix)
    {
        // Arrange
        var email = $"{emailPrefix}@someDomain.xyz";
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(email))
            .ReturnsAsync(default(User));

        // Act
        var result = await _sut.EmailExistsAsync(email);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(repo => repo.GetUserByEmailAsync(email), Times.Once);
    }

    [Theory]
    [AutoData]
    public async Task CreateUserAsync_ShouldReturnConflict_WhenEmailExists(string emailPrefix, string password)
    {
        // Arrange
        var email = $"{emailPrefix}@someDomain.xyz";
        var user = new User { Email = email, Password = password };
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.CreateUserAsync(user);

        // Assert
        Assert.Equal(UserStatus.Conflict, result);
    }

    [Theory]
    [AutoData]
    public async Task CreateUserAsync_ShouldReturnSuccess_WhenUserCreated(string emailPrefix, string password)
    {
        // Arrange
        var email = $"{emailPrefix}@someDomain.xyz";
        var user = new User { Email = email, Password = password };
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(user.Email))
            .ReturnsAsync(default(User));
        
        _mockUnitOfWork
            .Setup(uow => uow.CommitAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateUserAsync(user);

        // Assert
        Assert.Equal(UserStatus.Success, result);
        _mockUserRepository.Verify(repo => repo.InsertAsync(user), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
    }
}