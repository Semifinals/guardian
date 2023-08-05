namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthTokenDtoTests
{
    public readonly OAuthTokenDtoValidator Validator = new();

    public static OAuthTokenDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthTokenDto>(json)!;

    [TestMethod]
    [DataRow("authorization_code")]
    [DataRow("password")]
    [DataRow("client_credentials")]
    [DataRow("refresh_token")]
    public void GrantType_AcceptsCorrect(string type)
    {
        // Arrange
        var dto = Deserialize(@$"{{ ""grant_type"": ""{type}"" }}");
        
        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("code")]
    [DataRow("token")]
    [DataRow("invalid")]
    public void GrantType_DeniesInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{ ""grant_type"": ""{invalid}"" }}");

        // Act
        ValidationResult res = Validator.Validate(dto);
        
        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    [DataRow("code")]
    [DataRow("token")]
    public void ResponseType_AcceptsCorrect(string type)
    {
        // Arrange
        var dto = Deserialize(@$"{{ ""response_type"": ""{type}"" }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    [DataRow("authorization_code")]
    [DataRow("password")]
    [DataRow("client_credentials")]
    [DataRow("refresh_token")]
    [DataRow("invalid")]
    public void ResponseType_DeniesInvalid(string invalid)
    {
        // Arrange
        var dto = Deserialize(@$"{{ ""response_type"": ""{invalid}"" }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void OAuthTokenDto_AcceptsOnlyGrantType()
    {
        // Arrange
        var dto = Deserialize(@"{ ""grant_type"": ""authorization_code"" }");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
    
    [TestMethod]
    public void OAuthTokenDto_AcceptsOnlyResponseType()
    {
        // Arrange
        var dto = Deserialize(@"{ ""response_type"": ""code"" }");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
    
    [TestMethod]
    public void OAuthTokenDto_DeniesNeitherGrantNorResponseType()
    {
        // Arrange
        var dto = Deserialize("{ }");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void OAuthTokenDto_DeniesBothGrantAndResponseType()
    {
        // Arrange
        var dto = Deserialize(
            @"{ ""grant_type"": ""code"", ""response_type"": ""authorization_code"" }");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }
}
