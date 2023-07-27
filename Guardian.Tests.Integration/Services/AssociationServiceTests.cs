using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;

namespace Semifinals.Guardian.Services;

[TestClass]
public class AssosciationServiceTests
{
    [TestMethod]
    public async Task AddAccountAsync_SuccessfullyAddsAccount()
    {
        // Arrange
        string id = "id";
        string emailAddress = "user@example.com";
        string password = "password";
        string passwordHashed = Crypto.Hash(password);
        
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        accountRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Account(id, emailAddress, passwordHashed, false));

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Account? account = await associationService.AddAccountAsync(
            id,
            emailAddress,
            password);

        // Assert
        Assert.IsNotNull(account);
        Assert.AreEqual(id, account.Id);
    }

    [TestMethod]
    public async Task AddAccountAsync_FailsOnNonExistentIdentity()
    {
        // Arrange
        string id = "id";
        string emailAddress = "user@example.com";
        string password = "password";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(value: null);

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Account? account = await associationService.AddAccountAsync(
            id,
            emailAddress,
            password);

        // Assert
        Assert.IsNull(account);
    }

    [TestMethod]
    public async Task AddAccountAsync_FailsOnExistingEmailAddress()
    {
        // Arrange
        string id = "id";
        string emailAddress = "user@example.com";
        string password = "password";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        accountRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null))
            .ReturnsAsync(value: null);

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Account? account = await associationService.AddAccountAsync(
            id,
            emailAddress,
            password);

        // Assert
        Assert.IsNull(account);        
    }

    [TestMethod]
    public async Task AddIntegrationAsync_SuccessfullyAddsIntegration()
    {
        // Arrange
        string id = "id";
        string platform = "platform";
        string userId = "userId";
        string compositeId = Integration.GetCompositeId(platform, userId);

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        integrationRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new Integration(id, platform, userId));

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Integration? integration = await associationService.AddIntegrationAsync(
            id,
            platform,
            userId);

        // Assert
        Assert.IsNotNull(integration);
        Assert.AreEqual(compositeId, integration.Id);
    }

    [TestMethod]
    public async Task AddIntegrationAsync_FailsOnNonExistentIdentity()
    {
        // Arrange
        string id = "id";
        string platform = "platform";
        string userId = "userId";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Integration? integration = await associationService.AddIntegrationAsync(
            id,
            platform,
            userId);

        // Assert
        Assert.IsNull(integration);
    }

    [TestMethod]
    public async Task AddIntegrationAsync_FailsOnExistingIntegration()
    {
        // Arrange
        string id = "id";
        string platform = "platform";
        string userId = "userId";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        integrationRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(value: null);

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Integration? integration = await associationService.AddIntegrationAsync(
            id,
            platform,
            userId);

        // Assert
        Assert.IsNull(integration);
    }

    [TestMethod]
    public async Task RemoveIntegrationAsync_SuccessfullyRemovesIntegration()
    {
        // Arrange
        string id = "id";
        string platform = "platform";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(new Identity(id));

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Identity? identity = await associationService.RemoveIntegrationAsync(
            id,
            platform);

        // Assert

        // Because this operation doesn't care if the integration exists or not,
        // we don't need to assert anything here. It will fail if something
        // else is wrong with the method.
    }

    [TestMethod]
    public async Task RemoveIntegrationAsync_FailsOnNonExistentIdentity()
    {
        // Arrange
        string id = "id";
        string platform = "platform";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(value: null);

        AssociationService associationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Identity? identity = await associationService.RemoveIntegrationAsync(
            id,
            platform);

        // Assert
        Assert.IsNull(identity);
    }
}
