namespace MergeMaster.Api.GitHub;

public static class GitSha
{
    public static bool IsValid(string? input) =>
        input is not null && input.Length == 40 && input.All(char.IsAsciiHexDigitLower);
}