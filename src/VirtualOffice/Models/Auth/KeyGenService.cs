using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualOffice.Models
{
    public class KeyGenService : BackgroundService
    {
        private readonly KeyStore _keyStore;
        private readonly string _hostname;
        private readonly string _template;
        private readonly IEnumerable<string> _slacks;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KeyGenService> _logger;
        public KeyGenService(IOptions<Config> config, KeyStore keyStore, IHttpClientFactory httpClientFactory, ILogger<KeyGenService> logger)
        {
            _keyStore = keyStore;
            _hostname = config.Value.HostName ?? Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
            _slacks = config.Value.SlackWebHooks ?? Enumerable.Empty<string>();
            _template = config.Value.SlackWebHookTemplate;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTimeOffset.UtcNow;
                var next = now.Date.AddDays(1);

                var key = _keyStore.CreateNew(next);

                _logger.LogInformation($"LoginCode: {key.Code}");

                await NotifiyAsync(key.Code, stoppingToken);

                _keyStore.DeleteOld();

                await Task.Delay((int)(next - now).TotalMilliseconds, stoppingToken);
            }
        }

        public async Task NotifiyAsync(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(code))
                return;

            if (string.IsNullOrEmpty(_template))
                return;

            if (!_slacks.Any())
                return;


            var url = $"https://{_hostname}/login?code={code}";
            var client = _httpClientFactory.CreateClient();

            var tasks = _slacks.Select(slack =>
            {
                var json = _template.Replace("{url}", url);

                return client.PostAsync(slack, new StringContent(json,  Encoding.UTF8, "application/json"), cancellationToken);
            }).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}
