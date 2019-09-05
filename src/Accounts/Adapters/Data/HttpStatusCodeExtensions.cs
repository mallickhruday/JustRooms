using System.Net;

namespace Accounts.Adapters.Data
{
    /// <summary>
    /// Extension methods for dealing with HTTP status codes
    /// </summary>
    public static class HttpStatusCodeExtensions
    {
        /// <summary>
        /// Is this a success status code
        /// </summary>
        /// <param name="statusCode">The code to check for a success response</param>
        /// <returns></returns>
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        } 
    }
}