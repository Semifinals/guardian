using Microsoft.VisualBasic;
using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class IntegrationRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesIntegration()
    {
        // Arrange
        string id = "id";
        string identityId = "identityId";
        string platform = "platform";
            
        Integration integration = new(id, identityId, platform);

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
            id,
            identityId,
            platform);

        // Assert
        Assert.AreEqual(integration.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingIntegration()
    {
        // Arrange
        string id = "test";
        Integration integration = new(id, "identityId", "platform");

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
        Integration? res = await integrationRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(res);
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
        Integration? res = await integrationRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingIntegration()
    {
        // Arrange
        string id = "test";
        Integration integration = new(id, "identityId", "platform");

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
        Integration? res = await integrationRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNotNull(res);
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
        Integration? res = await integrationRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNull(res);
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
