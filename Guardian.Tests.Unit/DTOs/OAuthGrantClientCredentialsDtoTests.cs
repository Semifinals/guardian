namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthGrantClientCredentialsDtoTests
{
    public readonly OAuthGrantClientCredentialsDtoValidator Validator = new();

    public static OAuthGrantClientCredentialsDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthGrantClientCredentialsDto>(json)!;
    
    [TestMethod]
    public void GrantType_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""client_credentials""
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
            ""grant_type"": ""{invalid}""
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
        var dto = Deserialize("{ }");

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
            ""grant_type"": ""client_credentials"",
            ""scope"": ""example""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
}
