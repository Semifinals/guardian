using Semifinals.Guardian.Models;
using Semifinals.Guardian.Repositories;

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
    Task<Account?> AddAccountAsync(
        string id,
        string emailAddress,
        string password);

    /// <summary>
    /// Add a third-party integration to an account.
    /// </summary>
    /// <param name="id">The user's identity ID</param>
    /// <param name="platformId">The user's unique ID on the other platform</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <returns>The newly associated integration</returns>
    Task<Integration?> AddIntegrationAsync(
        string id,
        string platformId,
        string platform);

    /// <summary>
    /// Remove a third-party integration from an account.
    /// </summary>
    /// <param name="id">The user's identity ID</param>
    /// <param name="platformId">The user's unique ID on the other platform</param>
    /// <param name="platform">The name of the platform registering through</param>
    /// <returns>The integration that's association was removed</returns>
    Task<Integration?> RemoveIntegrationAsync(
        string id,
        string platformId,
        string platform);
}

public class AssociationService : IAssociationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IIdentityRepository _identityRepository;
    private readonly IIntegrationRepository _integrationRepository;

    public AssociationService(
        IAccountRepository accountRepository,
        IIdentityRepository identityRepository,
        IIntegrationRepository integrationRepository)
    {
        _accountRepository = accountRepository;
        _identityRepository = identityRepository;
        _integrationRepository = integrationRepository;
    }

    public async Task<Account?> AddAccountAsync(
        string id,
        string emailAddress,
        string password)
    {
        throw new NotImplementedException();
    }

    public async Task<Integration?> AddIntegrationAsync(
        string id,
        string platformId,
        string platform)
    {
        throw new NotImplementedException();
    }

    public async Task<Integration?> RemoveIntegrationAsync(
        string id,
        string platformId,
        string platform)
    {
        throw new NotImplementedException();
    }
}
