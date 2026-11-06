using MergeMaster.Api.Models;

namespace MergeMaster.Api.Data;


public interface IReportStore
{
    // find methods
    public Task<IReadOnlyList<Report>> FindAllAsync(CancellationToken ct);
    public Task<IReadOnlyList<Report>> FindAllFilteredPagesAsync(
        bool archived, bool? urgentOnly, List<string> authors, List<string> tags, 
        string? search, CancellationToken ct);
    public Task<bool> FindDuplicateAsync(string commit, CancellationToken ct);
    public Task<Report?> FindByIdAsync(int id, CancellationToken ct);
    public Task<List<string>> FindAuthorsAsync(bool archived, CancellationToken ct);
    public Task<List<string>> FindBranchesAsync(bool archived, CancellationToken ct);
    public Task<List<string>> FindCommitsAsync(bool archived, CancellationToken ct);
    public Task<List<string>> FindTagsAsync(bool archived, CancellationToken ct);

    // actions
    public Task<Report> AddAsync(ReportEntity newReport, CancellationToken ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct);
    public Task<bool> ArchiveAsync(int id, CancellationToken ct);
    public Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct);
}