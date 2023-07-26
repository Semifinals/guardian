using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;

namespace Semifinals.Guardian.Services;

public interface IRecoveryService
{
    /// <summary>
    /// Verify the email address associated with an account.
    /// </summary>
    /// <param name="id">The identity ID</param>
    /// <param name="emailAddress">The account's email address</param>
    /// <param name="recoveryCode">The recovery code used to validate the request</param>
    /// <returns>The updated account</returns>
    Task<Account?> VerifyAccountAsync(
        string id,
        string emailAddress,
        string recoveryCode);

    /// <summary>
    /// Change the email address of a first-party account. If no recovery code 
    /// is provided, email confirmation should be sent.
    /// </summary>
    /// <param name="oldEmailAddress">The user's old email address</param>
    /// <param name="newEmailAddress">The user's new email address</param>
    /// <param name="recoveryCode">The provided recovery code</param>
    /// <returns>The updated account</returns>
    Task<Account?> ChangeEmailAddressAsync(
        string id,
        string oldEmailAddress,
        string newEmailAddress,
        string? recoveryCode = null);

    /// <summary>
    /// Change a first-party account's password.
    /// </summary>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="oldPassword">The user's old password</param>
    /// <param name="newPassword">The user's new password</param>
    /// <returns>The updated account</returns>
    Task<Account?> ChangePasswordAsync(
        string id,
        string emailAddress,
        string oldPassword,
        string newPassword);

    /// <summary>
    /// Reset the password of a first-party account. If no recovery code is 
    /// provided, email confirmation should be sent.
    /// </summary>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="recoveryCode">The provided recovery code</param>
    /// <param name="newPassword">The user's new password</param>
    /// <returns>The updated account</returns>
    Task<Account?> ResetPasswordAsync(
        string id,
        string emailAddress,
        string newPassword,
        string recoveryCode);

    /// <summary>
    /// Delete a user by their identity ID.
    /// </summary>
    /// <param name="id">The identity ID of the user to delete</param>
    /// <returns>The deleted account</returns>
    Task<Identity?> DeleteByIdAsync(string id);
}

public class RecoveryService : IRecoveryService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IRecoveryCodeRepository _recoveryCodeRepository;

    public RecoveryService(
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IRecoveryCodeRepository recoveryCodeRepository)
    {
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _recoveryCodeRepository = recoveryCodeRepository;
    }

    public async Task<Account?> VerifyAccountAsync(
        string id,
        string emailAddress,
        string recoveryCode)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> ChangeEmailAddressAsync(
        string id,
        string oldEmailAddress,
        string newEmailAddress,
        string? recoveryCode = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> ChangePasswordAsync(
        string id,
        string emailAddress,
        string oldPassword,
        string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> ResetPasswordAsync(
        string id,
        string emailAddress,
        string newPassword,
        string recoveryCode)
    {
        throw new NotImplementedException();
    }

    public async Task<Identity?> DeleteByIdAsync(string id)
    {
        throw new NotImplementedException();
    }
}
