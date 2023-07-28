using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;
using Semifinals.Guardian.Utils.Exceptions;
using System.Security.Principal;

namespace Semifinals.Guardian.Services;

public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user through a first-party account. Doing so will send a 
    /// confirmation email.
    /// </summary>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>The newly created account</returns>
    /// <exception cref="AlreadyExistsException">Occurs when the account is already in use</exception>
    /// <exception cref="IdAlreadyExistsException">Occurs when the generated account ID is in use (should never occur)</exception>
    Task<Account> RegisterWithAccountAsync(string emailAddress, string password);

    /// <summary>
    /// Register a new user through a third-party integration.
    /// </summary>
    /// <param name="platformId">The user's unique ID on the other platform</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <returns>The newly created integration</returns>
    /// <exception cref="IdAlreadyExistsException">Occurs when the generated identity ID is in use (should never occur)</exception>
    /// <exception cref="AlreadyExistsException">Occurs when the integration is already in use</exception>
    Task<Integration> RegisterWithIntegrationAsync(string platformId, string platform);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public AuthenticationService(
        ILogger logger,
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
    }

    public async Task<Account> RegisterWithAccountAsync(
        string emailAddress,
        string password)
    {
        // Create account
        string passwordHashed = Crypto.Hash(password);

        Account account;
        try
        {
            account = await _accountRepository.CreateAsync(
                emailAddress,
                passwordHashed);
        }
        catch (AlreadyExistsException ex)
        {
            _logger.LogInformation(
                "Failed to create account {emailAddress} since it is already in use",
                emailAddress);

            throw ex;
        }

        // Create identity corresponding to account
        Identity identity;
        try
        {
            identity = await _identityRepository.CreateAsync(account.Id);
        }
        catch (IdAlreadyExistsException ex)
        {
            await _accountRepository.DeleteByIdAsync(account.Id);

            _logger.LogCritical(
                "Account {emailAddress} failed to create because of a problem creating its corresponding identity {id}",
                emailAddress,
                account.Id);

            throw ex;
        }

        // Return the newly created account
        _logger.LogInformation(
            "Account {emailAddress} successfully created with new ID {id}",
            emailAddress,
            account.Id);

        return account;
    }

    public async Task<Integration> RegisterWithIntegrationAsync(
        string userId,
        string platform)
    {
        // Create identity
        Identity identity;
        try
        {
            identity = await _identityRepository.CreateAsync();
        }
        catch (IdAlreadyExistsException ex)
        {
            _logger.LogCritical(
                "Failed to create generic identity on behalf of new integration {userId} ({platform})",
                userId,
                platform);

            throw ex;
        }

        // Create integration
        Integration integration;
        try
        {
            integration = await _integrationRepository.CreateAsync(identity.Id, platform, userId);
        }
        catch (AlreadyExistsException ex)
        {
            await _identityRepository.DeleteByIdAsync(identity.Id);

            _logger.LogCritical(
                "Identity {id} failed to create because of a problem creating its corresponding integration {userId} ({platform})",
                identity!.Id,
                userId,
                platform);

            throw ex;
        }

        // Link integration to account
        await _identityRepository.UpdateByIdAsync(identity.Id, new PatchOperation[]
        {
            PatchOperation.Add($"/integrations/{platform}", userId)
        });

        // Return the newly created integration
        _logger.LogInformation(
            "Integration {userId} ({platform}) successfully created with new ID {id}",
            userId,
            platform,
            identity.Id);

        return integration;
    }
}
