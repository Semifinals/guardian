using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Services;

public interface IRecoveryService
{
    /// <summary>
    /// Create a new recovery code for a given purpose.
    /// </summary>
    /// <param name="identityId">The identity ID of the user to generate a code for</param>
    /// <param name="type">The type of code to generate</param>
    /// <returns>The generated recovery code</returns>
    Task<RecoveryCode> CreateCodeAsync(
        string identityId,
        string type);

    /// <summary>
    /// Validate that a code exists and is valid.
    /// </summary>
    /// <param name="identityId">The identity ID the code belongs to</param>
    /// <param name="type">The type of code</param>
    /// <param name="code">The recovery code itself</param>
    /// <returns>Whether or not the code is valid</returns>
    Task<bool> ValidateCodeAsync(
        string identityId,
        string type,
        string code);

    /// <summary>
    /// Verify the email address associated with an account.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="emailAddress">The account's email address</param>
    /// <param name="recoveryCode">The recovery code used to validate the request</param>
    /// <returns>The updated account</returns>
    /// <exception cref="AccountNotFoundException">Occurs when the account doesn't exist</exception>
    /// <exception cref="InvalidRecoveryCodeException">Occurs when the recovery code is invalid</exception>
    Task<Account> VerifyAccountAsync(
        string identityId,
        string emailAddress,
        string recoveryCode);

    /// <summary>
    /// Change the email address of a first-party account.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="newEmailAddress">The user's new email address</param>
    /// <param name="recoveryCode">The provided recovery code</param>
    /// <returns>The updated account</returns>
    /// <exception cref="InvalidRecoveryCodeException">Occurs when the recovery code is invalid</exception>
    /// <exception cref="EmailAddressAlreadyExistsException">Occurs when the new email address is already in use</exception>
    Task<Account> ChangeEmailAddressAsync(
        string identityId,
        string newEmailAddress,
        string recoveryCode);

    /// <summary>
    /// Change a first-party account's password.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="oldPassword">The user's old password</param>
    /// <param name="newPassword">The user's new password</param>
    /// <returns>The updated account</returns>
    /// <exception cref="AccountNotFoundException">Occurs when the account does not exist (should never occur)</exception>
    /// <exception cref="InvalidPasswordException">Occurs when the old password is incorrect</exception>
    Task<Account> ChangePasswordAsync(
        string identityId,
        string emailAddress,
        string oldPassword,
        string newPassword);

    /// <summary>
    /// Reset the password of a first-party account.
    /// </summary>
    /// <param name="identityId">The user's identity ID</param>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="newPassword">The user's new password</param>
    /// <param name="recoveryCode">The provided recovery code</param>
    /// <returns>The updated account</returns>
    /// <exception cref="InvalidRecoveryCodeException">Occurs when the recovery code is invalid</exception>
    /// <exception cref="AccountNotFoundException">Occurs when the account does not exist (should never occur)</exception>
    Task<Account> ResetPasswordAsync(
        string identityId,
        string emailAddress,
        string newPassword,
        string recoveryCode);

    /// <summary>
    /// Delete a user by their identity ID.
    /// </summary>
    /// <param name="identityId">The identity ID of the user to delete</param>
    /// <param name="recoveryCode">The provided recovery code</param>
    /// <returns>The deleted identity</returns>
    /// <exception cref="InvalidRecoveryCodeException">Occurs when the recovery code is invalid</exception>
    /// <exception cref="IdentityNotFoundException">Occurs when the identity does not exist (should never occur)</exception>
    Task<Identity> DeleteByIdAsync(string identityId, string recoveryCode);
}

public class RecoveryService : IRecoveryService
{
    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;
    private readonly IRecoveryCodeRepository _recoveryCodeRepository;

    public RecoveryService(
        ILogger logger,
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository,
        IRecoveryCodeRepository recoveryCodeRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
        _recoveryCodeRepository = recoveryCodeRepository;
    }

    public async Task<RecoveryCode> CreateCodeAsync(
        string identityId,
        string type)
    {
        string code = Crypto.GenerateRandomString();

        RecoveryCode recoveryCode = await _recoveryCodeRepository.CreateAsync(
            identityId,
            code,
            type);

        return recoveryCode;
    }

    public async Task<bool> ValidateCodeAsync(
        string identityId,
        string type,
        string code)
    {
        RecoveryCode recoveryCode;
        try
        {
            recoveryCode = await _recoveryCodeRepository.GetByIdAsync(
                identityId,
                type);
        }
        catch (RecoveryCodeNotFoundException)
        {
            return false;
        }

        if (recoveryCode.Code != code)
            return false;

        return true;
    }

    public async Task<Account> VerifyAccountAsync(
        string identityId,
        string emailAddress,
        string recoveryCode)
    {
        // Validate the recovery code
        bool isValid = await ValidateCodeAsync(
            identityId,
            "VerifyEmailAddress",
            recoveryCode);

        if (!isValid)
        {
            _logger.LogInformation(
                "Unable to verify account {emailAddress} because the recovery code used was invalid",
                emailAddress);

            throw new InvalidRecoveryCodeException(recoveryCode);
        }

        // Verify account
        Account account;
        try
        {
            account = await _accountRepository.UpdateByIdAsync(
                identityId,
                new PatchOperation[]
                {
                    PatchOperation.Replace("/verified", true)
                });
        }
        catch (AccountNotFoundException ex)
        {
            _logger.LogInformation(
                "Unable to verify account {emailAddress} because it does not exist",
                emailAddress);

            throw ex;
        }

        // Remove the used recovery code and respond with updated account
        await _recoveryCodeRepository.DeleteByIdAsync(
            identityId,
            "VerifyEmailAddress");

        return account;
    }

    public async Task<Account> ChangeEmailAddressAsync(
        string identityId,
        string newEmailAddress,
        string recoveryCode)
    {
        // Validate the recovery code
        bool isValid = await ValidateCodeAsync(
            identityId,
            "ChangeEmailAddress",
            recoveryCode);

        if (!isValid)
        {
            _logger.LogInformation(
                "Unable to change account {identityId} email because the recovery code used was invalid",
                identityId);

            throw new InvalidRecoveryCodeException(recoveryCode);
        }

        // Update email address associated with the account
        Account account;
        try
        {
            account = await _accountRepository.UpdateByIdAsync(
                identityId,
                new PatchOperation[]
                {
                    PatchOperation.Replace("/emailAddress", newEmailAddress)
                });
        }
        catch (AccountNotFoundException)
        {
            _logger.LogInformation(
                "Unable to change account {identityId} to {newEmailAddress} as it is already in use",
                identityId,
                newEmailAddress);

            throw new EmailAddressAlreadyExistsException(newEmailAddress);
        }

        // Remove the used recovery code and respond with updated account
        await _recoveryCodeRepository.DeleteByIdAsync(
            identityId,
            "ChangeEmailAddress");

        return account;
    }

    public async Task<Account> ChangePasswordAsync(
        string identityId,
        string emailAddress,
        string oldPassword,
        string newPassword)
    {
        // Fetch the account
        Account account;
        try
        {
            account = await _accountRepository.GetByIdAsync(identityId);
        }
        catch (AccountNotFoundException ex)
        {
            _logger.LogInformation(
                "Unable to change account {emailAddress} password because it does not exist",
                emailAddress);

            throw ex;
        }

        // Check the old password is correct
        bool isValid = Crypto.Verify(oldPassword, account.PasswordHashed);
        
        if (!isValid)
        {
            _logger.LogInformation(
                "Unable to change account {emailAddress} password because the old password is incorrect",
                emailAddress);

            throw new InvalidPasswordException(oldPassword);
        }

        // Update the password and return the updated account
        string passwordHashed = Crypto.Hash(newPassword);

        Account? updatedAccount = await _accountRepository.UpdateByIdAsync(
            identityId,
            new PatchOperation[]
            {
                PatchOperation.Replace("/passwordHashed", passwordHashed)
            });

        return updatedAccount;
    }

    public async Task<Account> ResetPasswordAsync(
        string identityId,
        string emailAddress,
        string newPassword,
        string recoveryCode)
    {
        // Validate the recovery code
        bool isValid = await ValidateCodeAsync(
            identityId,
            "ResetPassword",
            recoveryCode);

        if (!isValid)
        {
            _logger.LogInformation(
                "Unable to change account {emailAddress} email because the recovery code used was invalid",
                emailAddress);

            throw new InvalidRecoveryCodeException(recoveryCode);
        }

        // Reset password
        string passwordHashed = Crypto.Hash(newPassword);

        Account account;
        try
        {
            account = await _accountRepository.UpdateByIdAsync(
                identityId,
                new PatchOperation[]
                {
                    PatchOperation.Replace("/passwordHashed", passwordHashed)
                });
        }
        catch (AccountNotFoundException ex)
        {
            _logger.LogInformation(
                "Unable to update password for account {emailAddress} because it does not exist",
                emailAddress);

            throw ex;
        }

        // Remove the used recovery code and respond with updated account
        await _recoveryCodeRepository.DeleteByIdAsync(
            identityId,
            "ResetPassword");

        return account;
    }

    public async Task<Identity> DeleteByIdAsync(
        string identityId,
        string recoveryCode)
    {
        // Validate the recovery code
        bool isValid = await ValidateCodeAsync(
            identityId,
            "DeleteAccount",
            recoveryCode);

        if (!isValid)
        {
            _logger.LogInformation(
                "Unable to delete account {identityId} email because the recovery code used was invalid",
                identityId);

            throw new InvalidRecoveryCodeException(recoveryCode);
        }

        // Fetch integrations connected to the account
        Identity identity;
        try
        {
            identity = await _identityRepository.GetByIdAsync(identityId);
        }
        catch (IdentityNotFoundException ex)
        {
            _logger.LogCritical(
                "Unable to delete account {identityId} email because their identity could not be found",
                identityId);

            throw ex;
        }

        List<(string, string)> integrationsToDelete = identity.Integrations
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToList();

        // Delete the account, identity, and associated integrations
        await _accountRepository.DeleteByIdAsync(identityId);
        await _identityRepository.DeleteByIdAsync(identityId);
        
        foreach ((string, string) integration in integrationsToDelete)
        {
            string platform = integration.Item1;
            string userId = integration.Item2;
            
            await _integrationRepository.DeleteByIdAsync(
                Integration.GetCompositeId(platform, userId));
        }

        // Remove the used recovery code and respond with deleted identity
        await _recoveryCodeRepository.DeleteByIdAsync(
            identityId,
            "ResetPassword");
        
        return identity;
    }
}
