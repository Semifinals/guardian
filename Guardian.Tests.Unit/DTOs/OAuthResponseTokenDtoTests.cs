namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthResponseTokenDtoTests
{
    public readonly OAuthResponseTokenDtoValidator Validator = new();

    public static OAuthResponseTokenDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthResponseTokenDto>(json)!;
    
    [TestMethod]
    public void ResponseType_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("code")]
    [DataRow("invalid")]
    public void ResponseType_DeniesInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""{invalid}"",
            ""client_id"": ""clientId""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);
        
        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void ResponseTypes_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""client_id"": ""clientId""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void ClientId_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void ClientId_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void RedirectUri_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId"",
            ""redirect_uri"": ""https://example.com""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("https:/malformed")]
    [DataRow("invalid")]
    public void RedirectUri_DeniesInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId"",
            ""redirect_uri"": ""{invalid}""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Scope_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId"",
            ""scope"": ""example""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void State_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""response_type"": ""token"",
            ""client_id"": ""clientId"",
            ""state"": ""example""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
}
