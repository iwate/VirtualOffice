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
                var now = DateTimeOffset.UtcNow.AddMinutes(10);
                var next = (DateTimeOffset)DateTime.SpecifyKind(now.Date.AddDays(1), DateTimeKind.Utc);

                var key = _keyStore.CreateNew(next);

                var url = $"https://{_hostname}/login?code={key.Code}";

                await NotifyLoggerAsync(url, stoppingToken);
                await NotifySlackAsync(url, stoppingToken);

                _keyStore.DeleteOld();

                await Task.Delay((int)(next - DateTimeOffset.UtcNow).TotalMilliseconds, stoppingToken);
            }
        }

        public Task NotifyLoggerAsync(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"LoginUrl: {url}");
            return Task.CompletedTask;
        }

        public async Task NotifySlackAsync(string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
                return;

            if (string.IsNullOrEmpty(_template))
                return;

            if (!_slacks.Any())
                return;

            var json = _template.Replace("{url}", url);
            var client = _httpClientFactory.CreateClient();

            var tasks = _slacks.Select(slack =>
            {
                return client.PostAsync(slack, new StringContent(json,  Encoding.UTF8, "application/json"), cancellationToken);
            }).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}
