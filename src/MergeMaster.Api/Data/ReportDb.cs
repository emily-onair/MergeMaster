using MergeMaster.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.Data;

public sealed class ReportDb(DbContextOptions<ReportDb> options) 
    : DbContext(options)
{
    public DbSet<ReportEntity> Reports => Set<ReportEntity>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<ReportEntity>(report =>
        {
            report.HasIndex(row => row.CommitSha)
                .IsUnique();
            report.HasQueryFilter(row => row.ArchivedAtUtc == null);
        });
    }
}