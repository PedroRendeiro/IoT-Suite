using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IoTSuite.Shared;

namespace IoTSuite.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ApplicationDbContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await _context.WeatherForecast.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<WeatherForecast>> Post([FromBody, BindRequired] WeatherForecastDTO weatherForecastDTO)
        {
            WeatherForecast weatherForecast = new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = weatherForecastDTO.TemperatureC,
                Summary = weatherForecastDTO.Summary
            };

            _context.WeatherForecast.Add(weatherForecast);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = weatherForecast.Id }, weatherForecast);
        }

        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<WeatherForecast>> Delete([FromRoute, Required] long Id)
        {
            var weatherForecast = await _context.WeatherForecast.FindAsync(Id);

            if (weatherForecast == null)
            {
                return NotFound();
            }

            _context.WeatherForecast.Remove(weatherForecast);
            await _context.SaveChangesAsync();

            return weatherForecast;
        }

        private bool WeatherForecastExists(long id)
        {
            return _context.WeatherForecast.Any(e => e.Id == id);
        }

    }
}