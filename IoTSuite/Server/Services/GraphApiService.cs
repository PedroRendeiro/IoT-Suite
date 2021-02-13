using IoTSuite.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTSuite.Server.Services
{
    public sealed class GraphApiService
    {
        private static string clientId;
        private static string tenantId;
        private static string clientSecret;

        public GraphApiService(IConfigurationSection configuration)
        {
            clientId = configuration["ClientId"];
            tenantId = configuration["TenantId"];
            clientSecret = configuration["ClientSecret"];
        }

        public static async Task<User> GetUser(string userId)
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                            .Create(clientId)
                            .WithTenantId(tenantId)
                            .WithClientSecret(clientSecret)
                            .Build();
            // Create an authentication provider by passing in a client application and graph scopes.
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            // Create a new instance of GraphServiceClient with the authentication provider.
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var request = await graphClient
                .Users[userId]
                .Request()
                .GetAsync();

            return request;
        }
    }
}
