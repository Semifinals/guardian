using Semifinals.Guardian.Models;

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
            
        Integration integration = new("id", identityId, platform);

        Mock<ILogger> logger = new();

        Mock<ItemResponse<Integration>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(integration);

        Mock<Container> container = new();
        container
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Integration>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IntegrationRepository integrationRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Integration res = await integrationRepository.CreateAsync(
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

        Mock<ItemResponse<Integration>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(integration);

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Integration>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

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

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Integration>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ThrowsAsync(new CosmosException("", 0, 0, "", 0));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

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

        Mock<ItemResponse<Integration>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(integration);

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Integration>(
                id,
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

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

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Integration>(
                id,
                It.IsAny<PartitionKey>(),
                It.IsAny<IReadOnlyList<PatchOperation>>(),
                null,
                default))
            .ThrowsAsync(new CosmosException("", 0, 0, "", 0));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

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

        Mock<ItemResponse<Integration>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(value: null!);

        Mock<Container> container = new();
        container
            .Setup(x => x.DeleteItemAsync<Integration>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

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
