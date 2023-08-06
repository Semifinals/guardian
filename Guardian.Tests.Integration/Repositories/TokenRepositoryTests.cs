using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class TokenRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesToken()
    {
        // Arrange
        string id = "id";
        string tkn = "token";
        int iat = 0;
        
        Token token = new(id, tkn, iat);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Token>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(token);

                container
                    .Setup(x => x.UpsertItemAsync(
                        It.IsAny<Token>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        TokenRepository tokenRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Token res = await tokenRepository.CreateAsync(
            id,
            tkn,
            iat);

        // Assert
        Assert.AreEqual(token.RefreshToken, res.RefreshToken);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingToken()
    {
        // Arrange
        string id = "id";
        int iat = 0;

        Token token = new(id, "token", iat);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Token>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(token);

                container
                    .Setup(x => x.ReadItemAsync<Token>(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        TokenRepository tokenRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Token res = await tokenRepository.GetByIdAsync(id, iat);
        
        // Assert
        Assert.AreEqual(token.RefreshToken, res.RefreshToken);
    }

    [TestMethod]
    public async Task GetAsync_FailsFetchingNonExistentToken()
    {
        // Arrange
        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<Token>(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        TokenRepository tokenRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Task<Token> res() => tokenRepository.GetByIdAsync("id", 0);

        // Assert
        await Assert.ThrowsExceptionAsync<TokenNotFoundException>(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingToken()
    {
        // Arrange
        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Token>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);

                container
                    .Setup(x => x.DeleteItemAsync<Token>(
                        It.IsAny<string>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        TokenRepository tokenRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        await tokenRepository.DeleteByIdAsync("id", 0);

        // Assert

        // Because this method doesn't care if the token exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
