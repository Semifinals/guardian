using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;
using Semifinals.Guardian.Utils;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Services;

public interface IAssociationService
{
    /// <summary>
    /// Add a first-party account to an account registered through an
    /// integration.
    /// </summary>
    /// <param name="id">The user's identity ID</param>
    /// <param name="emailAddress">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>The newly associated account</returns>
    /// <exception cref="IdentityNotFoundException">Occurs when the identity does not exist</exception>
    /// <exception cref="AlreadyExistsException">Occurs when the account details are already in use</exception>
    Task<Account> AddAccountAsync(
        string id,
        string emailAddress,
        string password);

    /// <summary>
    /// Add a third-party integration to an account.
    /// </summary>
    /// <param name="id">The user's identity ID</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <param name="userId">The user's unique ID on the other platform</param>
    /// <returns>The newly associated integration</returns>
    /// <exception cref="IdentityNotFoundException">Occurs when the identity does not exist</exception>
    /// <exception cref="IdAlreadyExistsException">Occurs when the integration is already in use</exception>
    Task<Integration> AddIntegrationAsync(
        string id,
        string platform,
        string userId);

    /// <summary>
    /// Remove a third-party integration from an account.
    /// </summary>
    /// <param name="id">The user's identity ID</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <returns>The resulting identity after the integration was removed</returns>
    /// <exception cref="IdentityNotFoundException">Occurs when the identity does not exist</exception>
    Task<Identity> RemoveIntegrationAsync(
        string id,
        string platform);
}

public class AssociationService : IAssociationService
{
    private readonly ILogger _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public AssociationService(
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

    public async Task<Account> AddAccountAsync(
        string id,
        string emailAddress,
        string password)
    {
        // Check the identity exists
        Identity identity;
        try
        {
            identity = await _identityRepository.GetByIdAsync(id);
        }
        catch (IdentityNotFoundException ex)
        {
            _logger.LogCritical(
                "Failed to add account {emailAddress} to non-existent identity {id}",
                emailAddress,
                id);

            throw ex;
        }

        // Create the account
        Account account;
        try
        {
            string passwordHashed = Crypto.Hash(password);
            
            account = await _accountRepository.CreateAsync(
                emailAddress,
                passwordHashed,
                id);
        }
        catch (AlreadyExistsException ex)
        {
            _logger.LogInformation(
                "Failed to add account {emailAddress} to identity {id} since it is already in use",
                emailAddress,
                id);

            throw ex;
        }

        // Return the newly created account
        _logger.LogInformation(
            "Successfully added account {emailAddress} to identity {id}",
            emailAddress,
            id);

        return account;
    }

    public async Task<Integration> AddIntegrationAsync(
        string id,
        string platform,
        string userId)
    {
        // Check the identity exists
        Identity identity;
        try
        {
            identity = await _identityRepository.GetByIdAsync(id);
        }
        catch (IdentityNotFoundException ex)
        {
            _logger.LogCritical(
                "Failed to add integration {userId} ({platform}) to non-existent identity {id}",
                userId,
                platform,
                id);
            
            throw ex;
        }

        // Create the integration
        Integration integration;
        try
        {
            integration = await _integrationRepository.CreateAsync(
                id,
                platform,
                userId);
        }
        catch (IdAlreadyExistsException ex)
        {
            _logger.LogInformation(
                "Failed to add integration {userId} ({platform}) to identity {id} since it is already in use",
                userId,
                platform,
                id);
            
            throw ex;
        }

        // Link integration to account
        await _identityRepository.UpdateByIdAsync(identity.Id, new PatchOperation[]
        {
            PatchOperation.Add($"/integrations/{platform}", userId)
        });

        // Return the newly created integration
        _logger.LogInformation(
            "Successfully added integration {userId} ({platform}) to identity {id}",
            userId,
            platform,
            id);

        return integration;
    }
    
    public async Task<Identity> RemoveIntegrationAsync(
        string id,
        string platform)
    {
        Identity identity;
        try
        {
            identity = await _identityRepository.UpdateByIdAsync(id, new PatchOperation[]
            {
            PatchOperation.Remove($"/integrations/{platform}")
            });
        }
        catch (IdentityNotFoundException ex)
        {
            _logger.LogInformation(
                "Failed to remove {platform} integration from identity {id}",
                platform,
                id);

            throw ex;
        }

        await _integrationRepository.DeleteByIdAsync(id);

        return identity;
    }
}
