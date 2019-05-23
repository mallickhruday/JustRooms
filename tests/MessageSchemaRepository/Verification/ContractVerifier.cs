using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MessageSchemaRepository.Verification
{
    public class ContractVerifier
    {
        private readonly HttpClient _client;

        public ContractVerifier(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<(bool, string)> GetVerificationResults(Uri baseUri, string path)
        {
            _client.BaseAddress = baseUri;
            var response = await _client.GetStringAsync(path);

            JObject result = JObject.Parse(response);

            string errors = null;
            bool hasErrors = result["failing"].Value<int>() > 0;
            if (hasErrors)
            {
                errors = result["errors"].ToString();
            }

            return (hasErrors, errors);
        }
    }

}