namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthGrantAuthorizationCodeDtoTests
{
    public readonly OAuthGrantAuthorizationCodeDtoValidator Validator = new();

    public static OAuthGrantAuthorizationCodeDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthGrantAuthorizationCodeDto>(json)!;    
    
    [TestMethod]
    public void GrantType_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""authorization_code"",
            ""code"": ""code"",
            ""client_id"": ""client_id""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("token")]
    [DataRow("invalid")]
    public void GrantType_DeniesInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""{invalid}"",
            ""code"": ""code"",
            ""client_id"": ""client_id""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void GrantType_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""code"": ""code"",
            ""client_id"": ""client_id""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);
        
        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Code_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""authorization_code"",
            ""code"": ""code"",
            ""client_id"": ""client_id""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void Code_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""authorization_code"",
            ""client_id"": ""client_id""
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
            ""grant_type"": ""authorization_code"",
            ""code"": ""code"",
            ""client_id"": ""client_id""
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
            ""grant_type"": ""authorization_code"",
            ""code"": ""code""
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
            ""grant_type"": ""authorization_code"",
            ""code"": ""code"",
            ""client_id"": ""client_id"",
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
            ""grant_type"": ""authorization_code"",
            ""code"": ""code"",
            ""client_id"": ""client_id"",
            ""redirect_uri"": ""{invalid}""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }
}
