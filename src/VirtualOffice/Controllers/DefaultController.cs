using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOffice.Models;

namespace VirtualOffice.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(IOptions<Config> config, ILogger<DefaultController> logger)
        {
            _config = config;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel { 
                SkyWayKey = _config.Value.SkyWayKey,
                FloorImage = _config.Value.FloorImage,
                Name = null,
                Icon = null
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
