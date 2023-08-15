using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;

namespace Semifinals.Guardian.Services;

[TestClass]
public class TokenServiceTests
{
    [TestMethod]
    public async Task GenerateAccessToken_GeneratesWithoutRefreshToken()
    {
        // Arrange
        string id = "id";

        Mock<ILogger> logger = new();
        Mock<IConfiguration> configuration = new();
        Mock<ITokenRepository> tokenRepository = new();
        
        Mock<IConfigurationSection> section = new Mock<IConfigurationSection>();
        section
            .Setup(x => x.Value)
            .Returns("some_secret");
        
        configuration
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(section.Object);

        TokenService tokenService = new(
            logger.Object,
            configuration.Object,
            tokenRepository.Object);

        // Act
        TokenResponse response = await tokenService.GenerateAccessToken(id, false);

        // Assert
        Dictionary<string, object> accessTokenPayload = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret("some_secret")
            .MustVerifySignature()
            .Decode<Dictionary<string, object>>(response.AccessToken);

        accessTokenPayload.TryGetValue("sub", out object? subject);
        Assert.IsNotNull(subject);
        Assert.AreEqual(id, subject.ToString());

        Assert.IsNull(response.RefreshToken);
    }

    [TestMethod]
    public async Task GenerateAccessToken_GeneratesWithRefreshToken()
    {
        // Arrange
        string id = "id";

        Mock<ILogger> logger = new();
        Mock<IConfiguration> configuration = new();
        Mock<ITokenRepository> tokenRepository = new();

        Mock<IConfigurationSection> section = new Mock<IConfigurationSection>();
        section
            .Setup(x => x.Value)
            .Returns("some_secret");

        configuration
            .Setup(x => x.GetSection(It.IsAny<string>()))
            .Returns(section.Object);

        TokenService tokenService = new(
            logger.Object,
            configuration.Object,
            tokenRepository.Object);

        // Act
        TokenResponse response = await tokenService.GenerateAccessToken(id, true);

        // Assert
        Dictionary<string, object> accessTokenPayload = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret("some_secret")
            .MustVerifySignature()
            .Decode<Dictionary<string, object>>(response.AccessToken);

        accessTokenPayload.TryGetValue("sub", out object? accessTokenSubject);
        Assert.IsNotNull(accessTokenSubject);
        Assert.AreEqual(id, accessTokenSubject.ToString());

        Assert.IsNotNull(response.RefreshToken);

        Dictionary<string, object> refreshTokenPayload = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret("some_secret")
            .MustVerifySignature()
            .Decode<Dictionary<string, object>>(response.RefreshToken);

        refreshTokenPayload.TryGetValue("sub", out object? refreshTokenSubject);
        Assert.IsNotNull(refreshTokenSubject);
        Assert.AreEqual(id, refreshTokenSubject.ToString());
    }
}
