namespace Azure_Ai_Search
{
    public class SearchRequest
    {
        public string Query { get; set; } = "*";
        public string? Filter { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }
        public string? OrderBy { get; set; }
        public string[]? Select { get; set; }
        public string[]? Facets { get; set; }
        public bool IncludeTotalCount { get; set; } = true;
    }
}
