using System.Security.Cryptography.X509Certificates;

namespace MergeMaster.Api.Services;

public sealed record SyncUpdate(
    bool Error,
    bool Flag,
    string Message
);