using Semifinals.Guardian.Mocks;
using Semifinals.Guardian.Models;
using System.Security.Principal;

namespace Semifinals.Guardian.Repositories;

[TestClass]
public class AccountRepositoryTests
{
    [TestMethod]
    public async Task CreateAsync_CreatesAccount()
    {
        // Arrange
        string emailAddress = "user@example.com";
        string passwordHashed = "abcd";
        bool verified = false;
        
        Account account = new("test", emailAddress, passwordHashed, verified);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Account>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(account);

                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Account>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Account res = await accountRepository.CreateAsync(
            emailAddress,
            passwordHashed);
        
        // Assert
        Assert.AreEqual(account.Id, res.Id);
    }

    [TestMethod]
    public async Task CreateAsync_CreatesWithExistingId()
    {
        // Arrange
        string id = "test";
        string emailAddress = "user@example.com";
        string passwordHashed = "abcd";
        bool verified = false;

        Account account = new(id, emailAddress, passwordHashed, verified);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Account>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(account);

                container
                    .Setup(x => x.CreateItemAsync(
                        It.IsAny<Account>(),
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Account res = await accountRepository.CreateAsync(
            emailAddress,
            passwordHashed,
            id);

        // Assert
        Assert.AreEqual(account.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_GetsExistingAccount()
    {
        // Arrange
        string id = "test";
        string emailAddress = "user@example.com";
        string passwordHashed = "abcd";
        bool verified = false;

        Account account = new(id, emailAddress, passwordHashed, verified);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Account>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(account);

                container
                    .Setup(x => x.ReadItemAsync<Account>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        Account? res = await accountRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(account.Id, res.Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_FailsGettingNonExistentAccount()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.ReadItemAsync<Account>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);
        
        // Act
        Account? res = await accountRepository.GetByIdAsync(id);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_UpdatesExistingAccount()
    {
        // Arrange
        string id = "test";
        string emailAddress = "user@example.com";
        string passwordHashed = "abcd";
        bool verified = false;

        Account account = new(id, emailAddress, passwordHashed, verified);

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Account>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(account);

                container
                    .Setup(x => x.PatchItemAsync<Account>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/passwordHashed", "efgh")
        };

        // Act
        Account? res = await accountRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNotNull(res);
        Assert.AreEqual(account.Id, res.Id);
    }

    [TestMethod]
    public async Task UpdateByIdAsync_FailsUpdatingNonExistentAccount()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                container
                    .Setup(x => x.PatchItemAsync<Account>(
                        id,
                        It.IsAny<PartitionKey>(),
                        It.IsAny<IReadOnlyList<PatchOperation>>(),
                        null,
                        default))
                    .ThrowsAsync(new CosmosException("", 0, 0, "", 0));
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);

        IEnumerable<PatchOperation> operations = new PatchOperation[]
        {
            PatchOperation.Add("/passwordHashed", "efgh")
        };

        // Act
        Account? res = await accountRepository.UpdateByIdAsync(id, operations);

        // Assert
        Assert.IsNull(res);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_DeletesPotentiallyExistingAccount()
    {
        // Arrange
        string id = "test";

        Mock<ILogger> logger = new();

        Mock<CosmosClient> cosmosClient = new CosmosClientMockBuilder()
            .SetupContainer(container =>
            {
                Mock<ItemResponse<Account>> itemResponse = new();
                itemResponse
                    .Setup(x => x.Resource)
                    .Returns(value: null!);

                container
                    .Setup(x => x.DeleteItemAsync<Account>(
                        id,
                        It.IsAny<PartitionKey>(),
                        null,
                        default))
                    .ReturnsAsync(itemResponse.Object);
            })
            .Create();

        AccountRepository accountRepository = new(
            logger.Object,
            cosmosClient.Object);

        // Act
        await accountRepository.DeleteByIdAsync(id);

        // Assert

        // Because this method doesn't care if the identity exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }
}
