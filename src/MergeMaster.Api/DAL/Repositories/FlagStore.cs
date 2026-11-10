using MergeMaster.Api.DAL.Models;
using MergeMaster.Api.Models;

namespace MergeMaster.Api.DAL.Repositories;

public class SyncFlagStore(ReportDb db)
{
    public async Task AddSingleAsync(SyncFlagEntity newFlag, CancellationToken ct)
    {
        db.SyncFlags.Add(newFlag);
        await db.SaveChangesAsync(ct);
    }

    // public async Task<int> AddManyAsync(SyncFlagTypes type, )
}