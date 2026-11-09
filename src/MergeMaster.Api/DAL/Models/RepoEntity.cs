namespace MergeMaster.Api.DAL.Models;

public class RepoEntity
{
    public int Id { get; init; }
    public string Owner { get; set; } = "";
    public string Repository { get; set; } = "";
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
}