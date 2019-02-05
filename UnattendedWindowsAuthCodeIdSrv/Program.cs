using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnattendedWindowsAuthCodeIdSrv
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientInfo = new ClientInfo
            {
                Id = ConfigurationManager.AppSettings["ClientId"],
                Secret = ConfigurationManager.AppSettings["ClientSecret"],
                RedirectUri = ConfigurationManager.AppSettings["RedirectUri"],
                AuthorityUri = ConfigurationManager.AppSettings["AuthorityUri"],
                Scopes = ConfigurationManager.AppSettings["Scopes"],
                Username = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"]
            };

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                Credentials = new NetworkCredential(clientInfo.Username, clientInfo.Password)
            };

            var identityClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(clientInfo.AuthorityUri),
            };

            // Initial Request will result in a redirect to /login
            var loginRedirect = await identityClient.GetAsync($"/connect/authorize?client_id={clientInfo.Id}&redirect_uri={clientInfo.RedirectUri}&response_type=code&scope={clientInfo.Scopes}");

            // Request to /login succeeds as we've set 'Credentials' correctly in the handler, 
            // and results in a redirect to /connect/authorize
            var authRedirect = await identityClient.GetAsync(loginRedirect.Headers.Location);

            // Auth request results in a redirect response to website, which we can parse to get the auth code.
            var codeResponse = await identityClient.GetAsync(authRedirect.Headers.Location);

            //Grab the code from the redirect back to DocProdUI
            var code = Regex.Match(codeResponse.Headers.Location.ToString(), "code=(?<Code>[^&]+)").Groups["Code"].Value;

            Console.WriteLine($"Your code is: '{code}'.");

            //Now, you can either get an access code by calling this method, or use the redirect URI from the codeResponse above as your selenium starting point.
            var accessToken = await GetAccessToken(identityClient, code, clientInfo);

            Console.WriteLine($"Your access token is: '{await accessToken.Content.ReadAsStringAsync()}'.");

            Console.ReadKey();
        }

        private static async Task<HttpResponseMessage> GetAccessToken(HttpClient httpClient, string authCode, ClientInfo client)
        {
            var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{client.Id}:{client.Secret}"));

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Auth}");

            var content = new StringContent($"grant_type=authorization_code&scope={client.Scopes}&code={authCode}&redirect_uri={client.RedirectUri}");

            content.Headers.Remove("Content-Type");

            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            return await httpClient.PostAsync("/connect/token", content);
        }
    }
}
