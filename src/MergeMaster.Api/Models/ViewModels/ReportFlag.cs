namespace MergeMaster.Api.Models.ViewModels;

public sealed record ReportFlag(DateTimeOffset DetectedAt, SyncFlagTypes Type);