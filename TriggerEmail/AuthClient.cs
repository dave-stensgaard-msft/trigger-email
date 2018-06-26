using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using TriggerEmail.Entities;
using TriggerEmail.Helpers;

namespace TriggerEmail
{
    internal class AuthClient
    {
        private static Configuration _configuration;

        public static HttpClient GetAuthenticatedClient(Configuration configuration)
        {
            _configuration = configuration;
            var keyVaultHelper = new KeyVaultHelper(configuration);

            var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            var client = new HttpClient(httpClientHandler);

            var token = GetAdToken(
                keyVaultHelper.ResolveKeyVaultPlaceholder(configuration.AppId),
                keyVaultHelper.ResolveKeyVaultPlaceholder(configuration.Secret),
                "https://msitaad.onmicrosoft.com/subscriptionsbyemail",
                "microsoft.onmicrosoft.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static string GetAdToken(string clientId, string clientSecretKey, string resource, string directory)
        {
            var authenticationContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}", directory));

            var credential = new ClientCredential(clientId, clientSecretKey);
            var result = authenticationContext.AcquireTokenAsync(resource, credential).Result;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the AD token");
            }

            return result.AccessToken;
        }
    }
}
