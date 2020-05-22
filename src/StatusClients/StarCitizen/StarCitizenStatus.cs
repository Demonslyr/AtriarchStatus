using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AtriarchStatus.StatusClients.StarCitizen
{
    public class StarCitizenStatus
    {
        private readonly IHttpClientFactory _clientFactory;
        StarCitizenStatus(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetGlobalStatus()
        {
            using var client = _clientFactory.CreateClient();
            using var response = await client.GetAsync("https://status.robertsspaceindustries.com/", HttpCompletionOption.ResponseHeadersRead);
            var content = await response.Content.ReadAsStringAsync();
            var match = Regex.Match(content, @"<div(?:[^>]*)global-status(?:[^>]*)><span>(?<StatusText>[^<]+)</span>");
            return match.Groups["StatusText"].Success ? match.Groups["StatusText"].Captures[0].Value : "";
        }
    }
}
