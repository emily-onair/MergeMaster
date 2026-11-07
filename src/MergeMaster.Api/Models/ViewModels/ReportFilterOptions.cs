namespace MergeMaster.Api.Models.ViewModels;

public sealed class ReportFilterOptions
{
    public bool IsUrgentOnly { get; set; }
    public bool IsDescending { get; set; }
    public List<string> SelectedAuthors { get; set; } = [];
    public List<string> SelectedTags { get; set; } = [];
    public string SearchValue { get; set; } = "";
    public int PageNumber { get; set; } = 1;
}
