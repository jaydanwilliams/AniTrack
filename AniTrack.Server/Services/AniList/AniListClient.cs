namespace AniTrack.Server.Services.AniList;

public class AniListClient
{
    private readonly HttpClient _http;

    public AniListClient(HttpClient http)
    {
        _http = http;
    }

    private const string PopularAnimeQuery = """
        query ($page: Int, $perPage: Int) {
          Page(page: $page, perPage: $perPage) {
            pageInfo { hasNextPage currentPage }
            media(type: ANIME, sort: POPULARITY_DESC, isAdult: false) {
              id
              title { romaji english }
              description(asHtml: false)
              coverImage { large }
              bannerImage
              format status season seasonYear
              episodes duration averageScore popularity
              startDate { year month day }
              endDate { year month day }
              genres
            }
          }
        }
        """;

    public async Task<AniListPage> GetPopularAnimeAsync(int page, int perPage, CancellationToken ct = default)
    {
        var body = new
        {
            query = PopularAnimeQuery,
            variables = new { page, perPage }
        };

        using var response = await _http.PostAsJsonAsync("", body, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AniListResponse>(cancellationToken: ct);
        return result?.Data?.Page
            ?? throw new InvalidOperationException("AniList returned no data.");
    }
}