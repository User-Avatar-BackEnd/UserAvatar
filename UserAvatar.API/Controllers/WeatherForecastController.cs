using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAvatar.BLL.Services;
using UserAvatar.DAL.Context;

namespace UserAvatar.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private AuthService _auth;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _auth = new AuthService();
        }

        [HttpPost("register/{email}/{password}")]
        public IActionResult Register(string email, string password)
        {
            return Ok(_auth.Register(email, password));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_auth.GetALlUsers());
        }
    }
}
