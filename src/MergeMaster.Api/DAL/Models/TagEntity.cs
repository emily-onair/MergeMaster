namespace MergeMaster.Api.DAL.Models;

public class TagEntity
{
    public int Id { get; init; }
    public string Name { get; set; } = "";
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
}