using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MessageSchemaRepository.Verification
{
    public class ContractVerifier : IDisposable
    {
        private readonly HttpClient _client;

        public ContractVerifier()
        {
            //we can't inject a factory into tests to use a pool, so it has to be this way
            _client = new HttpClient();
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

        public void Dispose()
        {
            _client.Dispose();
        }
    }

}