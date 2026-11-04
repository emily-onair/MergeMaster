namespace MergeMaster.Api.Extensions;

public static class DateTimeOffsetExtensions
{
    private const string DisplayFormat = "dd MMM, HH:mm";

    public static string ToDisplayDate(this DateTimeOffset date) =>
        date.ToString(DisplayFormat);
}
