using Semifinals.Guardian.Models;
using Semifinals.Guardian.Utils.Exceptions;

namespace Semifinals.Guardian.Repositories;

public interface IClientRepository
{
    /// <summary>
    /// Create a new oauth2 client.
    /// </summary>
    /// <returns>The newly created client</returns>
    /// <exception cref="IdAlreadyExistsException">Occurs when the ID is already in use (should never occur)</exception>
    Task<Client> CreateAsync();

    /// <summary>
    /// Get an oauth2 client by its ID.
    /// </summary>
    /// <param name="id">The ID to fetch by</param>
    /// <returns>The requested client</returns>
    /// <exception cref="ClientNotFoundException">Occurs when the client does not exist</exception>
    Task<Client> GetByIdAsync(string id);

    /// <summary>
    /// Update an oauth2 client by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to update</param>
    /// <param name="operations">The patch operations to perform</param>
    /// <returns>The updated client</returns>
    /// <exception cref="ClientNotFoundException">Occurs when the identity does not exist</exception>
    Task<Client> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations);

    /// <summary>
    /// Delete an oauth2 client by its ID.
    /// </summary>
    /// <param name="id">The ID of the client to delete</param>
    /// <returns>A task which will resolve once the client is deleted</returns>
    Task DeleteByIdAsync(string id);
}

public class ClientRepository : IClientRepository
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;

    private Task<Container> GetClientContainer()
    {
        return _cosmosClient.UseContainer(
            "identity-db",
            "clients");
    }

    public ClientRepository(ILogger logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task<Client> CreateAsync()
    {
        Container container = await GetClientContainer();

        string id = ShortId.Generate(new(useSpecialCharacters: false, length: 8));
        string secret = "";
        
        Client client;
        try
        {
            client = await container.CreateItemAsync<Client>(
                new(id, secret),
                new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation("Failed to create new client with ID {id}", id);

            throw new IdAlreadyExistsException(id);
        }

        _logger.LogInformation("Created new client with ID {id}", id);

        return client;
    }

    public async Task<Client> GetByIdAsync(string id)
    {
        Container container = await GetClientContainer();
        
        Client client;
        try
        {
            client = await container.ReadItemAsync<Client>(id, new(id));
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to get the client with ID {id}",
                id);

            throw new ClientNotFoundException(id);
        }

        _logger.LogInformation("Fetched the client with ID {id}", id);

        return client;
    }

    public async Task<Client> UpdateByIdAsync(
        string id,
        IEnumerable<PatchOperation> operations)
    {
        Container container = await GetClientContainer();

        Client client;
        try
        {
            client = await container.PatchItemAsync<Client>(
                id,
                new(id),
                (IReadOnlyList<PatchOperation>)operations);
        }
        catch (CosmosException)
        {
            _logger.LogInformation(
                "Unsuccessfully attempted to update the client with ID {id} changing the following properties: {properties}",
                id,
                string.Join(", ", operations.Select(o => o.Path)));

            throw new ClientNotFoundException(id);
        }

        _logger.LogInformation(
            "Updated the client with ID {id} changing the following properties: {properties}",
            id,
            string.Join(", ", operations.Select(o => o.Path)));
        
        return client;
    }

    public async Task DeleteByIdAsync(string id)
    {
        Container container = await GetClientContainer();

        await container.DeleteItemAsync<Client>(id, new(id));

        _logger.LogInformation("Deleted the client with ID {id}", id);
    }
}
