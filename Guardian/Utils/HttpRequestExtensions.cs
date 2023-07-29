namespace Semifinals.Guardian.Utils;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Get the body of the request formatted as JSON.
    /// </summary>
    /// <typeparam name="T">The type of the content to deserialize</typeparam>
    /// <param name="req">The HTTP request</param>
    /// <returns>The deserialized object</returns>
    /// <exception cref="InvalidOperationException">Occurs when the body is malformed or not present</exception>
    public static async Task<T> GetJsonBody<T>(this HttpRequest req)
    {
        var requestBody = await req.ReadAsStringAsync();
        
        T? requestObject = JsonSerializer.Deserialize<T>(requestBody);

        if (requestObject is null)
            throw new InvalidOperationException();

        return requestObject;
    }

    /// <summary>
    /// Get the body of a request formatted as JSON and validate it.
    /// </summary>
    /// <typeparam name="T">The type of the content to deserialize</typeparam>
    /// <typeparam name="V">The validator to test the content with</typeparam>
    /// <param name="req">The HTTP request</param>
    /// <returns>The deserialized and validated object</returns>
    /// <exception cref="InvalidOperationException">Occurs when the body is malformed or not present</exception>
    public static async Task<ValidatableRequest<T>> GetJsonBody<T, V>(
        this HttpRequest req)
        where V : AbstractValidator<T>, new()
    {
        T requestObject = await req.GetJsonBody<T>();
        
        ValidationResult validationResult = new V().Validate(requestObject);
        
        return new ValidatableRequest<T>(
            requestObject,
            validationResult.IsValid ? validationResult.Errors : null);
    }

    /// <summary>
    /// Create a bad request result from the invalid object.
    /// </summary>
    /// <typeparam name="T">The expected type of the request</typeparam>
    /// <param name="request">The validated request</param>
    /// <returns>A bad request object result with errors matching the validation failure</returns>
    public static BadRequestObjectResult ToBadRequest<T>(
        this ValidatableRequest<T> request)
    {
        return new BadRequestObjectResult(request.Errors.Select(e => new {
            field = e.PropertyName,
            error = e.ErrorMessage
        }));
    }

    /// <summary>
    /// Get the basic auth details from a HTTP request.
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <returns>The basic auth details if present</returns>
    public static BasicAuth? GetBasicAuth(this HttpRequest req)
    {
        if (!req.Headers.ContainsKey("Authorization"))
            return null;

        string authHeader = req.Headers["Authorization"][0];

        if (!authHeader.StartsWith("Basic "))
            return null;

        string token = authHeader["Basic ".Length..];
        byte[] tokenBytes = Convert.FromBase64String(token);
        string tokenDecoded = Encoding.Default.GetString(tokenBytes);

        if (tokenDecoded.Split(':').Length != 2)
            return null;

        string username = tokenDecoded.Split(':')[0];
        string password = tokenDecoded.Split(':')[1];

        return new(username, password);
    }
}

public class ValidatableRequest<T>
{
    public T Value { get; set; }
    
    public bool IsValid { get; set; }
    
    public IEnumerable<ValidationFailure> Errors { get; set; }

    public ValidatableRequest(
        T value,
        IEnumerable<ValidationFailure>? errors = null)
    {
        Value = value;
        IsValid = errors is null;
        Errors = errors ?? Array.Empty<ValidationFailure>();
    }
}

public class BasicAuth
{
    public string Username;

    public string Password;

    public BasicAuth(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
