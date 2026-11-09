using MergeMaster.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL;

public sealed class ReportDb(DbContextOptions<ReportDb> options) 
    : DbContext(options)
{
    public DbSet<ReportEntity> Reports => Set<ReportEntity>();
    public DbSet<AuthorEntity> Authors => Set<AuthorEntity>();
    public DbSet<BranchEntity> Branches => Set<BranchEntity>();
    public DbSet<RepoEntity> Repos => Set<RepoEntity>();
    public DbSet<TagEntity> Tags => Set<TagEntity>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<ReportEntity>(report =>
        {
            report.HasIndex(r => r.CommitSha)
                .IsUnique();
            // report.HasQueryFilter(r => r.ArchivedAtUtc == null);

            report.HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            report.HasOne(r => r.Branch)
                .WithMany()
                .HasForeignKey(r => r.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            report.HasMany(r => r.Tags)
                .WithMany()
                .UsingEntity(j => j.ToTable("ReportTags"));
        });

        model.Entity<RepoEntity>(repo => {
            repo.Property(r => r.Owner).UseCollation("NOCASE");
            repo.Property(r => r.Repository).UseCollation("NOCASE");
            repo.HasIndex(r => new { r.Owner, r.Repository })
                .IsUnique();
        });

        model.Entity<BranchEntity>(branch =>
        {
            branch.HasOne(b => b.Repo)
                .WithMany()
                .HasForeignKey(b => b.RepoId)
                .OnDelete(DeleteBehavior.Restrict);
            branch.HasIndex(b => new { b.RepoId, b.Name })
                .IsUnique();
        });

        model.Entity<AuthorEntity>(author =>
        {
            author.Property(b => b.Name).UseCollation("NOCASE");
            author.HasIndex(a => a.Name)
                .IsUnique();
        });

        model.Entity<TagEntity>(tag =>
        {
            tag.Property(r => r.Name).UseCollation("NOCASE");
            tag.HasIndex(t => t.Name).IsUnique();
        });
    }
}