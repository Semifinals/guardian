using Semifinals.Guardian.Models;

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
    Task<RecoveryCode> CreateAsync(string id, string code, string type);

    /// <summary>
    /// Fetch a recovery code to see if it exists and can be used.
    /// </summary>
    /// <param name="id">The ID of the identity the code is for</param>
    /// <returns>The requested recovery code if it exists</returns>
    Task<RecoveryCode?> GetByIdAsync(string id);

    /// <summary>
    /// Delete a recovery code by the given ID.
    /// </summary>
    /// <param name="id">The ID of the identity the code is for</param>
    /// <returns>A task which resolves when the recovery code is deleted</returns>
    Task DeleteByIdAsync(string id);
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
            defaultTimeToLive: 3600);
    }

    public RecoveryCodeRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task<RecoveryCode> CreateAsync(
        string id,
        string code,
        string type)
    {
        Container container = await GetRepositoryCodeContainer();
        
        RecoveryCode recoveryCode = await container.UpsertItemAsync<RecoveryCode>(
            new(id, code, type),
            new(id));

        _logger.LogInformation("Created new recovery code for ID {id}", id);

        return recoveryCode;
    }

    public async Task<RecoveryCode?> GetByIdAsync(string id)
    {
        Container container = await GetRepositoryCodeContainer();
        
        RecoveryCode recoveryCode;
        try
        {
            recoveryCode = await container.ReadItemAsync<RecoveryCode>(
                id,
                new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to fetch the recovery for ID {id}",
                id);

            return null;
        }

        _logger.LogInformation(
               "Successfully fetched the recovery code for ID {id}",
               id);

        return recoveryCode;
    }

    public async Task DeleteByIdAsync(string id)
    {
        Container container = await GetRepositoryCodeContainer();
        
        await container.DeleteItemAsync<RecoveryCode>(id, new(id));

        _logger.LogInformation("Deleted the recovery code for ID {id}", id);
    }
}
