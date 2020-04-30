using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOffice.Models;

namespace VirtualOffice.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IOptions<Config> _config;
        private readonly IUserResolver _userResolver;
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(IOptions<Config> config, IUserResolver userResolver, ILogger<DefaultController> logger)
        {
            _config = config;
            _userResolver = userResolver;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var info = await _userResolver.ResolveAsync();

            return View(new IndexViewModel { 
                SkyWayKey = _config.Value.SkyWayKey,
                FloorImage = _config.Value.FloorImage,
                Name = info?.Name,
                Icon = info?.Icon
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
