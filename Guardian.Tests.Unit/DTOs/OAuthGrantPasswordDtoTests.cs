namespace Semifinals.Guardian.DTOs;

[TestClass]
public class OAuthGrantPasswordDtoTests
{
    public readonly OAuthGrantPasswordDtoValidator Validator = new();

    public static OAuthGrantPasswordDto Deserialize(string json) =>
        JsonSerializer.Deserialize<OAuthGrantPasswordDto>(json)!;
    
    [TestMethod]
    public void GrantType_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""password"",
            ""username"": ""username"",
            ""password"": ""password""
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
            ""username"": ""username"",
            ""password"": ""password""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void GrantType_DenieNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""username"": ""username"",
            ""password"": ""password""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Username_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""password"",
            ""username"": ""username"",
            ""password"": ""password""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void Username_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""password"",
            ""password"": ""password""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);
        
        // Assert
        Assert.IsFalse(res.IsValid);
    }

    [TestMethod]
    public void Password_AcceptsCorrect()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""password"",
            ""username"": ""username"",
            ""password"": ""password""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }

    [TestMethod]
    public void Password_DeniesNull()
    {
        // Arrange
        var dto = Deserialize(@$"{{
            ""grant_type"": ""password"",
            ""username"": ""username""
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
            ""grant_type"": ""password"",
            ""username"": ""username"",
            ""password"": ""password"",
            ""scope"": ""example""
        }}");

        // Act
        ValidationResult res = Validator.Validate(dto);

        // Assert
        Assert.IsTrue(res.IsValid);
    }
}
