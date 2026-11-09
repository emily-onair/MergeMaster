using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Repositories;


public interface IReportStore
{
    // find methods
    public Task<IReadOnlyList<Report>> FindAllAsync(CancellationToken ct);
    public Task<PaginationResult> FindAllFilteredPagesAsync(
        bool archived, bool? descending, bool? urgentOnly, List<string> authors, 
        List<string> tags, string? search, int page, int pageSize, CancellationToken ct);
    public Task<bool> FindDuplicateAsync(string commit, CancellationToken ct);
    public Task<Report?> FindByIdAsync(int id, CancellationToken ct);

    public Task<DropdownOptionLists> FindDropdownOptionsAsync(bool archived, CancellationToken ct);
    
    // actions
    public Task<Report> AddAsync(NewReport newReport, CancellationToken ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct);
    public Task<bool> ArchiveAsync(int id, CancellationToken ct);
    public Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct);
}