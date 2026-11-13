using MergeMaster.Api.DAL;
using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Tests.Extensions;

public sealed class ReportQueryExtensionsTests : IDisposable // runs Dispose() after every test
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ReportDb> _options;

    public ReportQueryExtensionsTests() // constructor 
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<ReportDb>()
            .UseSqlite(_connection)
            .Options;

        using var db = CreateContext(); 
        db.Database.EnsureCreated(); // builds the tables
    }
    private ReportDb CreateContext() => new(_options);
    public void Dispose() => _connection.Dispose();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InArchive_ReturnsOnlyExpectedResults(bool archived)
    {
        // Arrange
        using (var db = CreateContext())
        {
            db.Reports.AddRange(
                NewReport(1, archived: true), 
                NewReport(2, archived: false));
            db.SaveChanges();
        }
        using var freshDb = CreateContext(); // no memory

        // Act
        var result = freshDb.Reports.InArchive(archived).ToList();

        // Assert
        var res = Assert.Single(result); // is one result (of the two)
        if (archived) Assert.NotNull(res.ArchivedAtUtc);
        else Assert.Null(res.ArchivedAtUtc);
    }

    [Fact]
    public void Populate_LoadsBranchRepoAndAuthor()
    {
        const int i = 1;
        using (var db = CreateContext())
        {
            db.Reports.AddRange(NewReport(i));
            db.SaveChanges();
        }
        using var freshDb = CreateContext();
        
        var report = freshDb.Reports.Populate().Single();

        // Assert
        Assert.NotNull(report.Branch);
        Assert.Equal($"Repository{i}", report.Branch.Repo.Repository);
        Assert.Equal($"Author{i}", report.Author.Name);
    }

    private static ReportEntity NewReport(int i, bool archived = false) => new()
    {
        CommitSha = $"commit{i}",
        Branch = new BranchEntity { Name = $"Name{i}", Repo = new RepoEntity { Owner = $"Owner{i}", Repository = $"Repository{i}" } },
        Author = new AuthorEntity { Name = $"Author{i}" },
        ArchivedAtUtc = archived ? DateTime.UtcNow : null
    };
}