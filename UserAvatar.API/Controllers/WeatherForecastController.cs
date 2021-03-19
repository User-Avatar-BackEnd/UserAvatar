using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.BLL.Services;
using UserAvatar.DAL.Context;
using UserAvatar.DAL.Entities;

namespace UserAvatar.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IAuthService _authService;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("register/{email}/{password}")]
        public IActionResult Register(string email, string password)
        {
            return Ok(_authService.Register(email, password));
        }

        [HttpGet("{email}")]
        public IActionResult GetByEmail(string email)
        {
            return Ok(_authService.GetUser(email));
        }
    }
}
