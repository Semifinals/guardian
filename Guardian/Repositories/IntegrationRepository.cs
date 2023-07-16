using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Repositories;

public interface IIntegrationRepository
{
    /// <summary>
    /// Create a new integration.
    /// </summary>
    /// <param name="identityId">The unique ID on the given platform</param>
    /// <param name="platform">The platform the integration comes from</param>
    /// <returns>The newly created integration</returns>
    Task<Integration> CreateAsync(string identityId, string platform);

    /// <summary>
    /// Get an integration by its ID.
    /// </summary>
    /// <param name="id">The ID of the integration to fetch</param>
    /// <returns>The requested integration</returns>
    Task<Integration?> GetByIdAsync(string id);

    /// <summary>
    /// Update a given integration by its ID.
    /// </summary>
    /// <param name="id">The ID of the integration to update</param>
    /// <param name="operations">The patch operations to perform</param>
    /// <returns>The updated integration</returns>
    Task<Integration?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations);

    /// <summary>
    /// Delete an integration by its ID.
    /// </summary>
    /// <param name="id">The ID of the integration to delete</param>
    /// <returns>A task which will resolve once the integration is deleted</returns>
    Task DeleteByIdAsync(string id);
}

public class IntegrationRepository : IIntegrationRepository
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Container Container => _cosmosClient.GetContainer(
        "identity-db",
        "integrations");

    public IntegrationRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }
    
    public async Task<Integration> CreateAsync(string identityId, string platform)
    {
        string id = ShortId.Generate(new(useSpecialCharacters: false, length: 8));
        
        Integration integration = await Container.CreateItemAsync<Integration>(
            new(id, identityId, platform),
            new(id));

        _logger.LogInformation("Created new integration with ID {id}", id);
        
        return integration;
    }
    
    public async Task<Integration?> GetByIdAsync(string id)
    {
        Integration integration;
        try
        {
            integration = await Container.ReadItemAsync<Integration>(id, new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the integration with ID {id}",
                id);

            return null;
        }

        _logger.LogInformation("Fetched the integration with ID {id}", id);

        return integration;
    }
    
    public async Task<Integration?> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations)
    {
        Integration integration;
        try
        {
            integration = await Container.PatchItemAsync<Integration>(
                id,
                new(id),
                (IReadOnlyList<PatchOperation>)operations);
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to update the integration with ID {id} changing the following properties: {properties}",
                id,
                string.Join(", ", operations.Select(o => o.Path)));

            return null;
        }

        _logger.LogInformation(
            "Updated the integration with ID {id} changing the following properties: {properties}",
            id,
            string.Join(", ", operations.Select(o => o.Path)));

        return integration;
    }

    public async Task DeleteByIdAsync(string id)
    {
        await Container.DeleteItemAsync<Integration>(id, new(id));

        _logger.LogInformation("Deleted the integration with ID {id}", id);
    }    
}
