namespace SearchService;


public class SearchParams
{
    public int PageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 4;
    public string SearchTerm { get; set; }
    public string Seller { get; set; }
    public string Winner { get; set; }
    public string OrderBy { get; set; }
    public string FilterBy { get; set; }
}