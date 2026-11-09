using MergeMaster.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

internal static class ReportQueryExtensions
{
    public static IQueryable<ReportEntity> Populate(this IQueryable<ReportEntity> query) => query
        .Include(r => r.Branch).ThenInclude(b => b.Repo)
        .Include(r => r.Author)
        .Include(r => r.Tags);

    public static IQueryable<ReportEntity> InArchive(this IQueryable<ReportEntity> query, bool archived) =>
        archived
            ? query.Where(r => r.ArchivedAtUtc != null)
            : query.Where(r => r.ArchivedAtUtc == null);
}
