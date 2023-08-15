using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Services;

[TestClass]
public class AuthenticationServiceTests
{
    [TestMethod]
    public async Task RegisterWithAccountAsync_RegistersNewAccount()
    {
        // Arrange
        string id = "id";
        string emailAddress = "user@example.com";
        string password = "password";
        string passwordHashed = "passwordHashed";
        
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        
        accountRepository
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), null))
            .ReturnsAsync(new Account(
                id,
                emailAddress,
                passwordHashed,
                false));
        
        identityRepository
            .Setup(x => x.CreateAsync(It.IsAny<string>()))
            .ReturnsAsync(new Identity(id));
        
        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Account account = await authenticationService.RegisterWithAccountAsync(
            emailAddress,
            password);

        // Assert
        Assert.AreEqual(id, account.Id);
        Assert.AreEqual(passwordHashed, account.PasswordHashed);
    }

    [TestMethod]
    public async Task RegisterWithAccountAsync_FailsWithExistingEmailAddress()
    {
        // Arrange
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        accountRepository
            .Setup(x => x.CreateAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                null))
            .ThrowsAsync(new AlreadyExistsException());

        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Task<Account> account() => authenticationService.RegisterWithAccountAsync(
            "user@example.com",
            "password");

        // Assert
        await Assert.ThrowsExceptionAsync<AlreadyExistsException>(account);
    }

    [TestMethod]
    public async Task LoginWithAccountAsync_LogsIntoExistingAccount()
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

        accountRepository
            .Setup(x => x.GetByEmailAddressAsync(emailAddress))
            .ReturnsAsync(new Account(id, emailAddress, passwordHashed, false));

        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Account account = await authenticationService.LoginWithAccountAsync(
            emailAddress,
            password);

        // Assert
        Assert.AreEqual(id, account.Id);
    }

    [TestMethod]
    public async Task LoginWithAccountAsync_FailsOnNonExistentAccount()
    {
        // Arrange
        string emailAddress = "user@example.com";
        string password = "password";
        
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        accountRepository
            .Setup(x => x.GetByEmailAddressAsync(emailAddress))
            .ThrowsAsync(new AccountNotFoundException(emailAddress));

        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Task<Account> account() => authenticationService.LoginWithAccountAsync(
            emailAddress,
            password);
        
        // Assert
        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(account);
    }    

    [TestMethod]
    public async Task RegisterWithIntegrationAsync_RegistersNewIntegration()
    {
        // Arrange
        string userId = "userId";
        string platform = "platform";
        string identityId = "identityId";
        
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.CreateAsync(null))
            .ReturnsAsync(new Identity(identityId));

        identityRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ReturnsAsync(new Identity(
                identityId,
                new Dictionary<string, string>() { { platform, userId } }));

        integrationRepository
            .Setup(x => x.CreateAsync(identityId, platform, userId))
            .ReturnsAsync(new Integration(identityId, platform, userId));

        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Integration integration = await authenticationService.RegisterWithIntegrationAsync(
            userId,
            platform);

        // Assert
        Assert.AreEqual(userId, integration.UserId);
    }

    [TestMethod]
    public async Task RegisterWithIntegrationAsync_FailsWithExistingIntegration()
    {
        // Arrange
        string userId = "userId";
        string platform = "platform";
        string identityId = "identityId";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();

        identityRepository
            .Setup(x => x.CreateAsync(null))
            .ReturnsAsync(new Identity(identityId));

        integrationRepository
            .Setup(x => x.CreateAsync(identityId, platform, userId))
            .ThrowsAsync(new AlreadyExistsException());

        AuthenticationService authenticationService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object);

        // Act
        Task<Integration> integration() => authenticationService.RegisterWithIntegrationAsync(
            userId,
            platform);

        // Assert
        await Assert.ThrowsExceptionAsync<AlreadyExistsException>(integration);
    }
}
