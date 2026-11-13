using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class AuthorStore(ReportDb db)
{
    public async Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Authors
            .Where(a => reports.Any(r => r.AuthorId == a.Id))
            .Select(a => a.Name)
            .OrderBy(n => n)
            .ToListAsync(ct);
    }

    public async Task<AuthorEntity> GetOrCreateAsync(string name, CancellationToken ct)
    {
        name = name.Trim();

        var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == name, ct);
        if (author is not null) return author;

        author = new AuthorEntity { Name = name, CreatedAtUtc = DateTime.UtcNow };
        db.Authors.Add(author);          // staged; ReportStore saves
        return author;
    }
}
