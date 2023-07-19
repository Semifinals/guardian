using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class RecoveryCodeRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesRecoveryCode()
    {
        // Arrange
        string id = "id";
        string code = "code";
        string type = "email";
        
        RecoveryCode recoveryCode = new(id, code, type);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<RecoveryCode>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(recoveryCode);

                container
                    .Setup(x => x.UpsertItemAsync(
                        It.IsAny<RecoveryCode>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        RecoveryCodeRepository recoveryCodeRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        RecoveryCode res = await recoveryCodeRepository.CreateAsync(
            id,
            code,
            type);
        
        // Assert
        Assert.AreEqual(recoveryCode.Code, res.Code);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingRecoveryCode()
    {
        // Arrange
        string id = "id";
        
        RecoveryCode recoveryCode = new(id, "code", "email");

        Mock<ILogger> logger = new();
        
        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<RecoveryCode>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(recoveryCode);

                container
                    .Setup(x => x.ReadItemAsync<RecoveryCode>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        RecoveryCodeRepository recoveryCodeRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        RecoveryCode? res = await recoveryCodeRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(recoveryCode.Code, res.Code);
    }

    [TestMethod]
    public async Task GetAsync_FailsFetchingNonExistentRecoveryCode()
    {
        // Arrange
        string id = "id";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<RecoveryCode>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        RecoveryCodeRepository recoveryCodeRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        RecoveryCode? res = await recoveryCodeRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingRecoveryCode()
    {
        // Arrange
        string id = "id";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<RecoveryCode>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);
                
                container
                    .Setup(x => x.DeleteItemAsync<RecoveryCode>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        RecoveryCodeRepository recoveryCodeRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        await recoveryCodeRepository.DeleteByIdAsync(id);

        // Assert

        // Because this method doesn't care if the code exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
