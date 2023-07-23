using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

public interface IIdentityRepository
{
    /// <summary>
    /// Create a new identity. Provide the ID if the indentity is being created
    /// for a new first-party account.
    /// </summary>
    /// <returns>The newly created identity</returns>
    Task<Identity?> CreateAsync(string? id = null);

    /// <summary>
    /// Get an identity by its ID.
    /// </summary>
    /// <param name="id">The ID to fetch by</param>
    /// <returns>The requested identity</returns>
    Task<Identity?> GetByIdAsync(string id);

    /// <summary>
    /// Update an identity by its ID.
    /// </summary>
    /// <param name="id">The ID of the identity to update</param>
    /// <param name="operations">The patch operations to perform</param>
    /// <returns>The updated identity</returns>
    Task<Identity?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations);

    /// <summary>
    /// Delete an identity by its ID.
    /// </summary>
    /// <param name="id">The ID of the identity to delete</param>
    /// <returns>A task which will resolve once the identity is deleted</returns>
    Task DeleteByIdAsync(string id);
}

public class IdentityRepository : IIdentityRepository
{    
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Task<Container> GetIdentityContainer()
    {
        return _cosmosClient.UseContainer(
            "identity-db",
            "identities");
    }

    public IdentityRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task<Identity?> CreateAsync(string? id = null)
    {
        Container container = await GetIdentityContainer();
        
        id ??= ShortId.Generate(new(useSpecialCharacters: false, length: 8));

        Identity identity;
        try
        {
            identity = await container.CreateItemAsync<Identity>(new(id), new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation("Failed to create new identity with ID {id}", id);

            return null;
        }

        _logger.LogInformation("Created new identity with ID {id}", id);
        
        return identity;
    }

    public async Task<Identity?> GetByIdAsync(string id)
    {
        Container container = await GetIdentityContainer();
        
        Identity identity;
        try
        {
            identity = await container.ReadItemAsync<Identity>(id, new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the identity with ID {id}",
                id);

            return null;
        }
        
        _logger.LogInformation("Fetched the identity with ID {id}", id);

        return identity;
    }

    public async Task<Identity?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations)
    {
        Container container = await GetIdentityContainer();
        
        Identity identity;
        try
        {
            identity = await container.PatchItemAsync<Identity>(
                id,
                new(id),
                (IReadOnlyList<PatchOperation>)operations);
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to update the identity with ID {id} changing the following properties: {properties}",
                id,
                string.Join(", ", operations.Select(o => o.Path)));
            
            return null;
        }

        _logger.LogInformation(
            "Updated the identity with ID {id} changing the following properties: {properties}",
            id,
            string.Join(", ", operations.Select(o => o.Path)));

        return identity;
    }
    
    public async Task DeleteByIdAsync(string id)
    {
        Container container = await GetIdentityContainer();
        
        await container.DeleteItemAsync<Identity>(id, new(id));

        _logger.LogInformation("Deleted the identity with ID {id}", id);
    }
}
