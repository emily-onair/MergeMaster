using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models.ViewModels;

namespace MergeMaster.Api.DAL.Interfaces;


public interface IReportStore
{
    // queries
    Task<List<ReportEntity>> GetActiveEntitiesAsync(CancellationToken ct);
    Task<PaginationResult> FilterAsync(FilterQueryParams queryParams, CancellationToken ct);
    Task<bool> CheckIfExistsAsync(string commit, CancellationToken ct);
    Task<Report?> FindByIdAsync(int id, CancellationToken ct);
    Task<DropdownOptionLists> GetDropdownOptionsAsync(bool archived, CancellationToken ct);
    
    // actions
    Task<Report> AddAsync(NewReport newReport, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<bool> ArchiveAsync(int id, CancellationToken ct);
    Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct);
}