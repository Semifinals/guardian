namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthGrantRefreshTokenDtoTests
{
    public readonly OAuthGrantRefreshTokenDtoValidator Validator = new();

    public static OAuthGrantRefreshTokenDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthGrantRefreshTokenDto>(json)!;
    
    [TestMethod]
    public void GrantType_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""refresh_token"",
            ""refresh_token"": ""refresh_token""
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
            ""refresh_token"": ""refresh_token""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    public void GrantType_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""refresh_token"": ""refresh_token""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void RefreshToken_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""refresh_token"",
            ""refresh_token"": ""refresh_token""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void RefreshToken_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""refresh_token""
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
            ""grant_type"": ""refresh_token"",
            ""refresh_token"": ""refresh_token"",
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
            ""grant_type"": ""refresh_token"",
            ""refresh_token"": ""refresh_token"",
            ""state"": ""example""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
}
