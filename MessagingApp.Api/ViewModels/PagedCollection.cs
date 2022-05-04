namespace MessagingApp.Api.ViewModels;

public class PagedCollection<T> where T : class, new()
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
}