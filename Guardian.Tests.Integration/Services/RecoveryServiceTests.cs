using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Services;

[TestClass]
public class RecoveryServiceTests
{
    [TestMethod]
    public async Task CreateCodeAsync_GeneratesOrReplacesCode()
    {
        // Arrange
        string identityId = "identityId";
        string type = "type";
        
        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.CreateAsync(
                identityId,
                It.IsAny<string>(),
                type))
            .ReturnsAsync(new RecoveryCode(identityId, "code", type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        RecoveryCode recoveryCode = await recoveryService.CreateCodeAsync(
            identityId,
            type);

        // Assert
        Assert.AreEqual(identityId, recoveryCode.IdentityId);
        Assert.AreEqual(type, recoveryCode.Type);
    }

    [TestMethod]
    public async Task ValidateCodeAsync_ValidatesExistingCode()
    {
        // Arrange
        string identityId = "identityId";
        string type = "type";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        bool isValid = await recoveryService.ValidateCodeAsync(
            identityId,
            type,
            code);

        // Assert
        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public async Task ValidateCodeAsync_FailsOnInvalidCode()
    {
        // Arrange
        string identityId = "identityId";
        string type = "type";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ThrowsAsync(new RecoveryCodeNotFoundException(identityId, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        bool isValid = await recoveryService.ValidateCodeAsync(
            identityId,
            type,
            code);
        
        // Assert
        Assert.IsFalse(isValid);
    }
    
    [TestMethod]
    public async Task VerifyAccountAsync_VerifiesExistingAccount()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string passwordHashed = "passwordHashed";
        string type = "VerifyEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();
        
        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                passwordHashed,
                true));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Account account = await recoveryService.VerifyAccountAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        Assert.AreEqual(identityId, account.Id);
        Assert.IsTrue(account.Verified);
    }

    [TestMethod]
    public async Task VerifyAccountAsync_FailsOnInvalidCode()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string type = "VerifyEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();
        
        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ThrowsAsync(new RecoveryCodeNotFoundException(identityId, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.VerifyAccountAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidRecoveryCodeException>(account);
    }

    [TestMethod]
    public async Task VerifyAccountAsync_FailsOnNonExistentAccount()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string type = "VerifyEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ThrowsAsync(new AccountNotFoundException(identityId));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.VerifyAccountAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(account);
    }

    [TestMethod]
    public async Task ChangeEmailAddressAsync_SuccessfullyChangesEmail()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string passwordHashed = "passwordHashed";
        string type = "ChangeEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                passwordHashed,
                true));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Account account = await recoveryService.ChangeEmailAddressAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        Assert.AreEqual(identityId, account.Id);
        Assert.AreEqual(emailAddress, account.EmailAddress);
    }

    [TestMethod]
    public async Task ChangeEmailAddressAsync_FailsOnInvalidCode()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string type = "ChangeEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ThrowsAsync(new RecoveryCodeNotFoundException(identityId, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ChangeEmailAddressAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidRecoveryCodeException>(account);
    }

    [TestMethod]
    public async Task ChangeEmailAddressAsync_FailsOnNonExistentAccountOrEmailInUse()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string type = "ChangeEmailAddress";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ThrowsAsync(new AccountNotFoundException(identityId));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ChangeEmailAddressAsync(
            identityId,
            emailAddress,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<EmailAddressAlreadyExistsException>(account);
    }

    [TestMethod]
    public async Task ChangePasswordAsync_SuccessfullyChangesPassword()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string oldPassword = "password1";
        string passwordHashed = Crypto.Hash(oldPassword);
        string newPassword = "password2";
        string newPasswordHashed = Crypto.Hash(newPassword);

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        accountRepository
            .Setup(x => x.GetByIdAsync(identityId))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                passwordHashed,
                true));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                newPasswordHashed,
                true));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Account account = await recoveryService.ChangePasswordAsync(
            identityId,
            emailAddress,
            oldPassword,
            newPassword);

        // Assert
        Assert.AreEqual(identityId, account.Id);
        Assert.AreEqual(newPasswordHashed, account.PasswordHashed);
    }

    [TestMethod]
    public async Task ChangePasswordAsync_FailsOnIncorrectOldPassword()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string oldPassword = "password1";
        string passwordHashed = Crypto.Hash(oldPassword);
        string newPassword = "password2";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        accountRepository
            .Setup(x => x.GetByIdAsync(identityId))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                passwordHashed,
                true));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ChangePasswordAsync(
            identityId,
            emailAddress,
            "invalidOldPassword",
            newPassword);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidPasswordException>(account);
    }

    [TestMethod]
    public async Task ChangePasswordAsync_FailsOnNonExistentAccount()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string oldPassword = "password1";
        string newPassword = "password2";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        accountRepository
            .Setup(x => x.GetByIdAsync(identityId))
            .ThrowsAsync(new AccountNotFoundException(identityId));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ChangePasswordAsync(
            identityId,
            emailAddress,
            oldPassword,
            newPassword);

        // Assert
        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(account);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_SuccessfullyResetsPassword()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string newPassword = "newPassword";
        string newPasswordHashed = Crypto.Hash(newPassword);
        string type = "ResetPassword";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ReturnsAsync(new Account(
                identityId,
                emailAddress,
                newPasswordHashed,
                true));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Account account = await recoveryService.ResetPasswordAsync(
            identityId,
            emailAddress,
            newPassword,
            code);

        // Assert
        Assert.AreEqual(identityId, account.Id);
        Assert.AreEqual(newPasswordHashed, account.PasswordHashed);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_FailsOnInvalidCode()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string newPassword = "newPassword";
        string type = "ResetPassword";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ThrowsAsync(new RecoveryCodeNotFoundException(identityId, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ResetPasswordAsync(
            identityId,
            emailAddress,
            newPassword,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidRecoveryCodeException>(account);
    }

    [TestMethod]
    public async Task ResetPasswordAsync_FailsOnNonExistentAccount()
    {
        // Arrange
        string identityId = "identityId";
        string emailAddress = "user@example.com";
        string newPassword = "newPassword";
        string type = "ResetPassword";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        accountRepository
            .Setup(x => x.UpdateByIdAsync(
                identityId,
                It.IsAny<IEnumerable<PatchOperation>>()))
            .ThrowsAsync(new AccountNotFoundException(identityId));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Account> account() => recoveryService.ResetPasswordAsync(
            identityId,
            emailAddress,
            newPassword,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<AccountNotFoundException>(account);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_SuccessfullyDeletesAccount()
    {
        // Arrange
        string identityId = "identityId";
        string type = "DeleteAccount";
        string code = "code";

        Dictionary<string, string> integrations = new()
        {
            { "platform1", "userId1" },
            { "platform2", "userId2" }
        };

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        identityRepository
            .Setup(x => x.GetByIdAsync(identityId))
            .ReturnsAsync(new Identity(identityId, integrations));

        int integrationsDeleted = 0;
        integrationRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<string>()))
            .Callback(() => integrationsDeleted++);

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Identity identity = await recoveryService.DeleteByIdAsync(
            identityId,
            code);

        // Assert
        Assert.AreEqual(identityId, identity.Id);
        Assert.AreEqual(integrations.Count, integrationsDeleted);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_FailsOnInvalidCode()
    {
        // Arrange
        string identityId = "identityId";
        string type = "DeleteAccount";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ThrowsAsync(new RecoveryCodeNotFoundException(identityId, type));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Identity> identity() => recoveryService.DeleteByIdAsync(
            identityId,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidRecoveryCodeException>(identity);
    }

    [TestMethod]
    public async Task DeleteByIdAsync_FailsOnNonExistentAccount()
    {
        // Arrange
        string identityId = "identityId";
        string type = "DeleteAccount";
        string code = "code";

        Mock<ILogger> logger = new();
        Mock<IAccountRepository> accountRepository = new();
        Mock<IIdentityRepository> identityRepository = new();
        Mock<IIntegrationRepository> integrationRepository = new();
        Mock<IRecoveryCodeRepository> recoveryCodeRepository = new();

        recoveryCodeRepository
            .Setup(x => x.GetByIdAsync(identityId, type))
            .ReturnsAsync(new RecoveryCode(identityId, code, type));

        identityRepository
            .Setup(x => x.GetByIdAsync(identityId))
            .ThrowsAsync(new IdentityNotFoundException(identityId));

        RecoveryService recoveryService = new(
            logger.Object,
            accountRepository.Object,
            identityRepository.Object,
            integrationRepository.Object,
            recoveryCodeRepository.Object);

        // Act
        Task<Identity> identity() => recoveryService.DeleteByIdAsync(
            identityId,
            code);

        // Assert
        await Assert.ThrowsExceptionAsync<IdentityNotFoundException>(identity);
    }
}
