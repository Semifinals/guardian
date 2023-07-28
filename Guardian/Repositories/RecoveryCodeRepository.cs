using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

public interface IRecoveryCodeRepository
{
    /// <summary>
    /// Create a new recovery code.
    /// </summary>
    /// <param name="id">The ID of the identity the code is for</param>
    /// <param name="code">The recovery code</param>
    /// <param name="type">The type of recovery it can perform</param>
    /// <returns>The newly created recovery code</returns>
    Task<RecoveryCode> CreateAsync(string identityId, string code, string type);

    /// <summary>
    /// Fetch a recovery code to see if it exists and can be used.
    /// </summary>
    /// <param name="identityId">The ID of the identity the code is for</param>
    /// <param name="type">The type of recovery it can perform</param>
    /// <returns>The requested recovery code if it exists</returns>
    /// <exception cref="RecoveryCodeNotFoundException">Occurs when the recovery code does not exist</exception>
    Task<RecoveryCode> GetByIdAsync(string identityId, string type);

    /// <summary>
    /// Delete a recovery code by the given ID.
    /// </summary>
    /// <param name="identityId">The ID of the identity the code is for</param>
    /// <param name="type">The type of recovery it can perform</param>
    /// <returns>A task which resolves when the recovery code is deleted</returns>
    Task DeleteByIdAsync(string identityId, string type);
}

public class RecoveryCodeRepository : IRecoveryCodeRepository
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Task<Container> GetRepositoryCodeContainer()
    {
        return _cosmosClient.UseContainer(
            "identity-db",
            "recoveryCodes",
            partitionKeyPath: "/identityId",
            defaultTimeToLive: 3600);
    }

    public RecoveryCodeRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task<RecoveryCode> CreateAsync(
        string identityId,
        string code,
        string type)
    {
        Container container = await GetRepositoryCodeContainer();
        
        RecoveryCode recoveryCode = await container.UpsertItemAsync<RecoveryCode>(
            new(identityId, code, type),
            new(identityId));

        _logger.LogInformation("Created new recovery code for ID {id}", identityId);

        return recoveryCode;
    }

    public async Task<RecoveryCode> GetByIdAsync(string identityId, string type)
    {
        Container container = await GetRepositoryCodeContainer();
        
        RecoveryCode recoveryCode;
        try
        {
            recoveryCode = await container.ReadItemAsync<RecoveryCode>(
                RecoveryCode.GetCompositeId(identityId, type),
                new(identityId));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to fetch the recovery for ID {id}",
                identityId);

            throw new RecoveryCodeNotFoundException(identityId, type);
        }

        _logger.LogInformation(
               "Successfully fetched the recovery code for ID {id}",
               identityId);
        
        return recoveryCode;
    }

    public async Task DeleteByIdAsync(string identityId, string type)
    {
        Container container = await GetRepositoryCodeContainer();
        
        await container.DeleteItemAsync<RecoveryCode>(
            RecoveryCode.GetCompositeId(identityId, type),
            new(identityId));

        _logger.LogInformation(
            "Deleted the recovery code for ID {id}",
            identityId);
    }
}
