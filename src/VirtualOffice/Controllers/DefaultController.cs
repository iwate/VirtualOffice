using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualOffice.Models;

namespace VirtualOffice.Controllers
{
    public class DefaultController : Controller
    {
        private readonly KeyStore _keyStore;
        private readonly IOptions<Config> _config;
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(KeyStore keyStore, IOptions<Config> config, ILogger<DefaultController> logger)
        {
            _keyStore = keyStore;
            _config = config;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View(new IndexViewModel { 
                SkyWayKey = _config.Value.SkyWayKey,
                FloorImage = _config.Value.FloorImage,
                Name = HttpContext.User.FindFirstValue(ClaimTypes.Name),
                Icon = HttpContext.User.FindFirstValue("icon")
            });
        }

        [HttpGet("/login")]
        public IActionResult Login(string code)
        {
            if (!_keyStore.Validate(code))
                return Unauthorized();

            return View(new LoginViewModel { Code = code });
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!_keyStore.Validate(model.Code))
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(model);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Name),
                new Claim("icon", model.Icon),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            
            var authProperties = new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.Date.AddDays(1) };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
