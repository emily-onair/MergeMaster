namespace MergeMaster.Api.DAL.Models;

public class BranchEntity
{
    public int Id { get; init; }
    public string Name { get; set; } = "";
    public int RepoId { get; set; } // foreign key points to RepoEntity
    public RepoEntity Repo { get; set; } = null!;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
}