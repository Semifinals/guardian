using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class IntegrationRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesIntegration()
    {
        // Arrange
        string identityId = "identityId";
        string platform = "platform";
        string userId = "userId";
            
        Integration integration = new(identityId, platform, userId);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Integration>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(integration);
                
                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Integration>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Integration res = await integrationRepository.CreateAsync(
            identityId,
            platform,
            userId);

        // Assert
        Assert.AreEqual(integration.Id, res.Id);
    }
    
    [TestMethod]
    public async Task CreateAsync_FailsRecreatingExistingIntegration()
    {
        // Arrange
        string identityId = "identityId";
        string platform = "platform";
        string userId = "userId";

        Integration integration = new(identityId, platform, userId);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Integration>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Task<Integration> res() => integrationRepository.CreateAsync(
            identityId,
            platform,
            userId);

        // Assert
        await Assert.ThrowsExceptionAsync<AlreadyExistsException>(res);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingIntegration()
    {
        // Arrange
        string platform = "platform";
        string userId = "userId";
        string id = Integration.GetCompositeId(platform, userId);
        
        Integration integration = new("identityId", platform, userId);

        Mock<ILogger> logger = new();      

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Integration>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(integration);
                
                container
                    .Setup(x => x.ReadItemAsync<Integration>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Integration res = await integrationRepository.GetByIdAsync(id);

        // Assert
        Assert.AreEqual(integration.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_FailsGettingNonExistentIntegration()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<Integration>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Task<Integration> res() => integrationRepository.GetByIdAsync(id);

        // Assert
        await Assert.ThrowsExceptionAsync<IntegrationNotFoundException>(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingIntegration()
    {
        // Arrange
        string platform = "platform";
        string userId = "userId";
        string id = Integration.GetCompositeId(platform, userId);
        
        Integration integration = new("identityId", platform, userId);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Integration>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(integration);
                
                container
                    .Setup(x => x.PatchItemAsync<Integration>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();        

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/identityId", "newId")
        };

        // Act
        Integration res = await integrationRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.AreEqual(integration.Id, res.Id);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_FailsUpdatingNonExistentIntegration()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.PatchItemAsync<Integration>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/identityId", "newId")
        };

        // Act
        Task<Integration> res() => integrationRepository.UpdateByIdAsync(id, operations);

        // Assert
        await Assert.ThrowsExceptionAsync<IntegrationNotFoundException>(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingIntegration()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Integration>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);

                container
                    .Setup(x => x.DeleteItemAsync<Integration>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        await integrationRepository.DeleteByIdAsync(id);

        // Assert

        // Because this method doesn't care if the integration exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
