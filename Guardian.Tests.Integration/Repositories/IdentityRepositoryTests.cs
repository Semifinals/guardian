using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

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

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Identity>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(identity);
                
                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Identity>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

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

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Identity>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(identity);

                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Identity>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Identity res = await identityRepository.CreateAsync(id);

        // Assert
        Assert.AreEqual(identity.Id, res.Id);
    }

    [TestMethod]
    public async Task CreateAsync_FailsRecreatingExistingIdentity()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Identity>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Task<Identity> res() => identityRepository.CreateAsync(id);

        // Assert
        await Assert.ThrowsExceptionAsync<IdAlreadyExistsException>(res);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingIdentity()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Identity>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(identity);

                container
                    .Setup(x => x.ReadItemAsync<Identity>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Identity res = await identityRepository.GetByIdAsync(id);

        // Assert
        Assert.AreEqual(identity.Id, res.Id);        
    }

    [TestMethod]
    public async Task GetByIdAsync_FailsGettingNonExistentIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();        

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<Identity>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Task<Identity> res() => identityRepository.GetByIdAsync(id);

        // Assert
        await Assert.ThrowsExceptionAsync<IdentityNotFoundException>(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingIdentity()
    {
        // Arrange
        string id = "test";
        Identity identity = new(id);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Identity>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(identity);

                container
                    .Setup(x => x.PatchItemAsync<Identity>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };
        
        // Act
        Identity res = await identityRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.AreEqual(identity.Id, res.Id);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_FailsUpdatingNonExistentIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.PatchItemAsync<Identity>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        IdentityRepository identityRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };

        // Act
        Task<Identity> res() => identityRepository.UpdateByIdAsync(id, operations);

        // Assert
        await Assert.ThrowsExceptionAsync<IdentityNotFoundException>(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingIdentity()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Identity>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);

                container
                    .Setup(x => x.DeleteItemAsync<Identity>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

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
