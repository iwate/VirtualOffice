using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VirtualOffice.Models
{
    public class MSGraphUserResolver : IUserResolver
    {
        private const string TOKEN_KEY = "X-MS-TOKEN-AAD-ACCESS-TOKEN";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public MSGraphUserResolver(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<UserInfo> ResolveAsync()
        {
            var context = _httpContextAccessor.HttpContext;

            if (!context.Request.Headers.ContainsKey(TOKEN_KEY))
                return null;

            var token = context.Request.Headers[TOKEN_KEY].ToString();

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (!jwt.Audiences.Contains("https://graph.microsoft.com"))
                return null;

            var name = GetName(jwt);
            var icon = await GetPhotoAsync(token);

            return new UserInfo { Name = name, Icon = icon };
        }

        public string GetName(JwtSecurityToken jwt)
        {
            var lastName = jwt.Claims.Where(o => o.Type == "family_name").FirstOrDefault();
            var firstName = jwt.Claims.Where(o => o.Type == "given_name").FirstOrDefault();
            var name = $"{lastName} {firstName}";

            return !string.IsNullOrWhiteSpace(name) ? name : null;
        }

        public async ValueTask<string> GetPhotoAsync(string token)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/photos/48x48/$value");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var photo = await response.Content.ReadAsByteArrayAsync();

            var mime = response.Content.Headers.ContentType.MediaType;
            var base64 = Convert.ToBase64String(photo);
            
            return $"data:${mime};base64,{base64}";
        }
    }
}