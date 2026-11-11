using MergeMaster.Api.Extensions;
using MergeMaster.Api.Models;

namespace MergeMaster.Tests;

public class SyncFlagMessagesTests
{
    [Theory]
    [InlineData(SyncFlagTypes.RepoDoesNotExist, "Repository does not exist on GitHub")]
    [InlineData(SyncFlagTypes.BranchDoesNotExist, "Branch does not exist on GitHub")]
    [InlineData(SyncFlagTypes.CommitAlreadyMerged, "Commit has already been merged")]
    public void ToMessage_ReturnsExpectedText(SyncFlagTypes flag, string expected)
    {
        Assert.Equal(expected, flag.ToMessage());
    }

    [Fact]
    public void ToMessage_HasSpecificMessage_ForEveryEnumValue()
    {
        // Fails if someone adds a new SyncFlagTypes value but forgets a message for it.
        foreach (var flag in Enum.GetValues<SyncFlagTypes>())
        {
            Assert.NotEqual("Out of sync with GitHub", flag.ToMessage());
        }
    }
}
