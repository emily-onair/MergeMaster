using MergeMaster.Api.Models;

namespace MergeMaster.Api.Data;


public interface IReportStore
{
    public Task<Report> AddAsync(ReportEntity newReport, CancellationToken ct);
    public Task<bool> FindDuplicateAsync(string commit, CancellationToken ct);
    public Task<IReadOnlyList<Report>> FindAllAsync(CancellationToken ct);
    public Task<Report?> FindByIdAsync(int id, CancellationToken ct);
    public Task<IReadOnlyList<Report>> FindUrgentOnlyAsync(CancellationToken ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct);
    public Task<bool> ArchiveAsync(int id, CancellationToken ct);
    public Task<bool> RestoreFromArchiveAsync(int id, CancellationToken ct);
}