namespace MergeMaster.Api.Services.GitHub.Models;

public sealed record ResultCompare(string Status, int AheadBy, int BehindBy);