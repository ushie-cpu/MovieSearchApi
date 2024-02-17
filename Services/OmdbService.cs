using Entity.Models;
using Newtonsoft.Json;
namespace Services
{
    public class OmdbService
    {
        private const string ApiKey = "2c10191a";
        private const string OmdbApiUrl = "http://www.omdbapi.com/";

        private readonly HttpClient _httpClient;

        public OmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(OmdbApiUrl);
        }

        private async Task<Movie> SendRequestAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Movie>(responseBody);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error: {ex.Message}");
            }
        }

        public Task<Movie> SearchMovieByIdAsync(string id)
        {
            string endpoint = $"?apikey={ApiKey}&i={id}";
            return SendRequestAsync(endpoint);
        }

        public Task<Movie> SearchMovieByTitleAsync(string title, string type, string year)
        {
            string apiUrl = $"?apikey={ApiKey}&t={title}";

            if (!string.IsNullOrEmpty(type))
                apiUrl += $"&type={type}";

            if (!string.IsNullOrEmpty(year))
                apiUrl += $"&y={year}";

            return SendRequestAsync(apiUrl);
        }
    }
}