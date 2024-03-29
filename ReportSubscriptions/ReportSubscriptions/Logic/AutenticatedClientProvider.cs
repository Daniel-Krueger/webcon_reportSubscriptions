﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ReportSubscriptions.Model;

namespace ReportSubscriptions.Logic
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Copied from https://github.com/WEBCON-BPS/RestApi-DataImporter
    /// </remarks>
    public class AutenticatedClientProvider
    {
        private readonly Configuration _config;
        private HttpClient _client;

        public AutenticatedClientProvider(Configuration config)
        {
            _config = config;
        }

        public async Task<HttpClient> GetClientAsync()
        {
            if (_client == null)
                _client = await PrepareClient();

            return _client;
        }

        private async Task<HttpClient> PrepareClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_config.PortalUrl);

            var auth = new LoginModel(_config.ClientId, _config.ClientSecret, _config.ImpersonationLogin);

            var request = await client.PostAsync("api/login", new StringContent(JsonConvert.SerializeObject(auth), Encoding.UTF8, "application/json"));
            var response = await request.Content.ReadAsStringAsync();

            if (!request.IsSuccessStatusCode)
                throw new ApiException().CreateEx(response);

            var responseObject = JObject.Parse(response);
            var accessToken = (string)responseObject["token"];

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return client;
        }
    }
}
