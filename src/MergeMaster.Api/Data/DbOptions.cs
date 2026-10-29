using System.ComponentModel.DataAnnotations;

namespace MergeMaster.Api.Data;

public sealed class DbOptions
{
    public const string Section = "Database";

    [Required] public required string Path { get; init; }
}