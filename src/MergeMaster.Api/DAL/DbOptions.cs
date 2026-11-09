using System.ComponentModel.DataAnnotations;

namespace MergeMaster.Api.DAL;

public sealed class DbOptions
{
    public const string Section = "Database";

    [Required] public required string Path { get; init; }
}