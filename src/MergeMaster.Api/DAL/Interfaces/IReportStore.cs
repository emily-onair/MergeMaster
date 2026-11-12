using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Interfaces;


public interface IReportStore
{
    // queries
    public Task<List<ReportEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    public Task<PaginationResult> FilterAsync(
        bool archived, bool? descending, bool? urgentOnly, List<string> authors, 
        List<string> tags, string? search, int page, int pageSize, CancellationToken ct);
    public Task<bool> CheckIfExistsAsync(string commit, CancellationToken ct);
    public Task<Report?> FindByIdAsync(int id, CancellationToken ct);
    public Task<DropdownOptionLists> GetDropdownOptionsAsync(bool archived, CancellationToken ct);
    
    // actions
    public Task<Report> AddAsync(NewReport newReport, CancellationToken ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct);
    public Task<bool> ArchiveAsync(int id, CancellationToken ct);
    public Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct);
}