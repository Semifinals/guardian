using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class IdentityRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesIdentity()
    {
        // Arrange
        Identity identity = new("test");
        
        Mock<ILogger> logger = new();

        Mock<ItemResponse<Identity>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(identity);

        Mock<Container> container = new();
        container
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Identity>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Identity res = await identityRepository.CreateAsync();

        // Assert
        Assert.AreEqual(identity.Id, res.Id);
    }

    [TestMethod]
    public async Task CreateAsync_CreatesWithExistingId()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<ItemResponse<Identity>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(identity);

        Mock<Container> container = new();
        container
            .Setup(x => x.CreateItemAsync(
                It.IsAny<Identity>(),
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Identity res = await identityRepository.CreateAsync(id);

        // Assert
        Assert.AreEqual(identity.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingIdentity()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<ItemResponse<Identity>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(identity);

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Identity>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Identity? res = await identityRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(identity.Id, res.Id);        
    }

    [TestMethod]
    public async Task GetByIdAsync_FailsGettingNonExistentIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<Container> container = new();
        container
            .Setup(x => x.ReadItemAsync<Identity>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ThrowsAsync(new CosmosException("", 0, 0, "", 0));

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Identity? res = await identityRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingIdentity()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<ItemResponse<Identity>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(identity);

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Identity>(
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

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };
        
        // Act
        Identity? res = await identityRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(identity.Id, res.Id);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_FailsUpdatingNonExistentIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<Container> container = new();
        container
            .Setup(x => x.PatchItemAsync<Identity>(
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

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };

        // Act
        Identity? res = await identityRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<ItemResponse<Identity>> itemResponse = new();
        itemResponse
            .Setup(x => x.Resource)
            .Returns(value: null!);

        Mock<Container> container = new();
        container
            .Setup(x => x.DeleteItemAsync<Identity>(
                id,
                It.IsAny<PartitionKey>(),
                null,
                default))
            .ReturnsAsync(itemResponse.Object);

        Mock<CosmosClient> cosmosClient = new();
        cosmosClient
            .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(container.Object);

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        await identityRepository.DeleteByIdAsync(id);

        // Assert
        
        // Because this method doesn't care if the identity exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
