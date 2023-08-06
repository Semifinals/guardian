using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class ClientRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesClient()
    {
        // Arrange
        Client client = new("id", "secret");

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Client>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(client);

                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Client>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Client res = await clientRepository.CreateAsync();

        // Assert
        Assert.AreEqual(client.Id, res.Id);
    }

    [TestMethod]
    public async Task CreateAsync_CreatesWithExistingId()
    {
        // Arrange
        Client client = new("id", "secret");

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Client>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(client);

                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Client>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Client res = await clientRepository.CreateAsync();

        // Assert
        Assert.AreEqual(client.Id, res.Id);
    }

    [TestMethod]
    public async Task CreateAsync_FailsRecreatingExistingClient()
    {
        // Arrange
        Client client = new("id", "secret");

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Client>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Task<Client> res() => clientRepository.CreateAsync();

        // Assert
        await Assert.ThrowsExceptionAsync<IdAlreadyExistsException>(res);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingClient()
    {
        // Arrange
        string id = "test";
        Client client = new(id, "secret");

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Client>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(client);

                container
                    .Setup(x => x.ReadItemAsync<Client>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Client res = await clientRepository.GetByIdAsync(id);

        // Assert
        Assert.AreEqual(client.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_FailsGettingNonExistentClient()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<Client>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Task<Client> res() => clientRepository.GetByIdAsync(id);

        // Assert
        await Assert.ThrowsExceptionAsync<ClientNotFoundException>(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingClient()
    {
        // Arrange
        string id = "test";
        Client client = new(id, "secret");

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Client>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(client);

                container
                    .Setup(x => x.PatchItemAsync<Client>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };

        // Act
        Client res = await clientRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.AreEqual(client.Id, res.Id);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_FailsUpdatingNonExistentClient()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.PatchItemAsync<Client>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/integrations/platform", "specialid")
        };

        // Act
        Task<Client> res() => clientRepository.UpdateByIdAsync(id, operations);

        // Assert
        await Assert.ThrowsExceptionAsync<ClientNotFoundException>(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingClient()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Client>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);

                container
                    .Setup(x => x.DeleteItemAsync<Client>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        ClientRepository clientRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        await clientRepository.DeleteByIdAsync(id);

        // Assert

        // Because this method doesn't care if the client exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
