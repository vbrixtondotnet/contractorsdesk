using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("quickbooks")]
    public class QuickBooksController : Controller
    {
		private readonly IConfiguration configuration;
		private readonly IQuickBooksService quickBooksService;

		public QuickBooksController(
			IConfiguration configuration,
			IQuickBooksService quickBooksService)
        {
            this.configuration = configuration;
			this.quickBooksService = quickBooksService;
        }

        [Route("connect")]
        public IActionResult Connect()
        {
			var qbSettings = configuration.GetSection("QuickBooks");
			var clientId = qbSettings.GetSection("ClientId").Value;

			var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/quickbooks/callback";

			var authorizationEndpoint = qbSettings.GetSection("AuthorizationEndpoint").Value;
			var scopes = qbSettings.GetSection("Scopes").Get<string[]>()
				?? throw new InvalidOperationException("QuickBooks Scopes are not configured.");
			var state = GenerateState();
			var url = $"{authorizationEndpoint}?client_id={clientId}&response_type=code&scope={scopes[0]}&redirect_uri={redirectUri}&state={state}";

			return Redirect(url);
		}

        [Route("callback")]
        public async Task<IActionResult> Callback(string code, string state, string realmId)
        {
			if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(realmId))
			{
				return BadRequest("Authorization failed");
			}

			var tokenResponse = await ExchangeCodeForToken(code);
			
			if (tokenResponse != null)
			{
				await quickBooksService.SaveAccessToken(tokenResponse, realmId);
			}

            return Redirect("/");
        }

		private string GenerateState()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
		}

		private async Task<TokenResponsePayload> ExchangeCodeForToken(string code)
		{
			var qbSettings = configuration.GetSection("QuickBooks");
			var tokenEndpoint = qbSettings.GetSection("TokenEndpoint").Value;
			var clientId = qbSettings.GetSection("ClientId").Value;
			var clientSecret = qbSettings.GetSection("ClientSecret").Value;
			//var redirectUri = qbSettings.GetSection("RedirectUri").Value
			//	?? throw new InvalidOperationException("QuickBooks RedirectUri are not configured.");

			var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/quickbooks/callback";

			using var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
				Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

			request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "grant_type", "authorization_code" },
				{ "code", code },
				{ "redirect_uri", redirectUri }
			});

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<TokenResponsePayload>(responseContent)
				?? throw new NullReferenceException("QuickBooks request get token returned null.");
		}
	}
}
