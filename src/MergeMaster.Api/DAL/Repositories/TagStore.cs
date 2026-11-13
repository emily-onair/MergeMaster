using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MergeMaster.Api.DAL.Repositories;

public class TagStore(ReportDb db)
{
    public async Task<List<string>> GetNamesAsync(bool archived, CancellationToken ct)
    {
        var reports = db.Reports.InArchive(archived);

        return await db.Tags
            .Where(t => reports.Any(r => r.Tags.Any(rt => rt.Id == t.Id)))
            .Select(t => t.Name)
            .OrderBy(n => n)
            .ToListAsync(ct);
    }
    public async Task<List<TagEntity>> GetOrCreateAsync(string tags, CancellationToken ct)
    {
        var tagNames = tags.Split(",");
        var wanted = tagNames
            .Select(n => n.Trim())
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (wanted.Count == 0) return [];

        var existing = await db.Tags.Where(t => wanted.Contains(t.Name)).ToListAsync(ct);

        var created = wanted
            .Except(existing.Select(t => t.Name), StringComparer.OrdinalIgnoreCase)
            .Select(n => new TagEntity { Name = n, CreatedAtUtc = DateTime.UtcNow })
            .ToList();
        db.Tags.AddRange(created);

        return [.. existing, .. created];
    }
}