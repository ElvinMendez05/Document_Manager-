using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace Document_Manager.Presentation.Services
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ProtectedLocalStorage _storage;

        public JwtAuthorizationMessageHandler(ProtectedLocalStorage storage)
        {
            _storage = storage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var tokenResult = await _storage.GetAsync<string>("authToken");

            if (tokenResult.Success)
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenResult.Value);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
