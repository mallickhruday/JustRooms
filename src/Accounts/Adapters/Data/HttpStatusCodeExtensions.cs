using System.Net;

namespace Accounts.Adapters.Data
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        } 
    }
}