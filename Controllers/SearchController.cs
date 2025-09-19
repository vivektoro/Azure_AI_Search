using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure_Ai_Search;
using Microsoft.AspNetCore.Mvc;

namespace AzureAISearchDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly SearchClient _searchClient;

    public SearchController(IConfiguration config)
    {
        _config = config;

        string endpoint = _config["SearchServiceUri"]!;
        string apiKey = _config["SearchApiKey"]!;
        string indexName = _config["SearchIndexName"]!;

        _searchClient = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));
    }

    [HttpPost("query")]
    public async Task<IActionResult> QueryAsync([FromBody] SearchRequest request)
    {
        try
        {
            var options = new SearchOptions
            {
                IncludeTotalCount = request.IncludeTotalCount
            };

            if (request.Top.HasValue) options.Size = request.Top.Value;
            if (request.Skip.HasValue) options.Skip = request.Skip.Value;
            if (!string.IsNullOrEmpty(request.Filter)) options.Filter = request.Filter;
            if (!string.IsNullOrEmpty(request.OrderBy)) options.OrderBy.Add(request.OrderBy);
            if (request.Select != null) foreach (var field in request.Select) options.Select.Add(field);
            if (request.Facets != null) foreach (var facet in request.Facets) options.Facets.Add(facet);

            var response = await _searchClient.SearchAsync<SearchDocument>(request.Query, options);

            var results = response.Value.GetResults()
                .Select(r => new { Score = r.Score, Document = r.Document });

            return Ok(new
            {
                request.Query,
                response.Value.TotalCount,
                Results = results
            });
        }
        catch (Exception ex)
        {
            var bytes = System.IO.File.ReadAllBytes("Error.jpg");
            return File(bytes, "image/jpg");
        }
    }
}
