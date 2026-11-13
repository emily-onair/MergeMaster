using MergeMaster.Api.Services.GitHub;

namespace MergeMaster.Tests;

public class GitShaTests
{
    [Fact]
    public void IsValid_ReturnsTrue_ForFortyLowercaseHexChars()
    {
        // Arrange
        var sha = "a94a8fe5ccb19ba61c4c0873d391e987982fbbd3";

        // Act
        var result = GitSha.IsValid(sha);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]                                          // missing
    [InlineData("")]                                            // empty
    [InlineData("a94a8fe")]                                     // short SHA
    [InlineData("A94A8FE5CCB19BA61C4C0873D391E987982FBBD3")]    // uppercase
    [InlineData("g94a8fe5ccb19ba61c4c0873d391e987982fbbd3")]    // 'g' isn't hex
    public void IsValid_ReturnsFalse_ForBadInput(string? input)
    {
        Assert.False(GitSha.IsValid(input));
    }
}
