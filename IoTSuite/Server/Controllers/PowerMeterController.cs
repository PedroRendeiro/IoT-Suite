using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTSuite.Shared;
using Microsoft.AspNetCore.SignalR;
using IoTSuite.Server.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using IoTSuite.Server.Helpers;
using IoTSuite.Server.Services;
using IoTSuite.Shared.Wrappers;
using IoTSuite.Shared.Extensions;

namespace IoTSuite.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class PowerMeterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalR> _hubContext;
        private readonly IUriService _uriService;

        public PowerMeterController(ApplicationDbContext context, IHubContext<SignalR> hubContext, IUriService uriService = null)
        {
            _context = context;
            _hubContext = hubContext;
            _uriService = uriService;
        }

        // GET: PowerMeter/Total
        [HttpGet("Total")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<PowerMeterTotal>>>> GetPowerMeterTotal([FromQuery] PowerMeterTotalPaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value;

            if (filter.MinDate.HasValue)
            {
                Console.WriteLine(filter.MinDate.Value);
            }
            
            (var pagedData, var totalCount) = await filter.Process(_context.PowerMeterTotal, _context.Thing, new Guid(userId));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: PowerMeter/Total/5
        [HttpGet("Total/{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<PowerMeterTotal>>> GetPowerMeterTotal(long Id)
        {
            var measure = await _context.PowerMeterTotal.FindAsync(Id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(new Response<PowerMeterTotal>(measure));
        }

        // PUT: PowerMeter/Total/5
        [HttpPut("Total/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutPowerMeterTotal(long Id, PowerMeterTotalDTO powerMeterTotalDTO)
        {
            var powerMeterTotal = await _context.PowerMeterTotal.FindAsync(Id);

            if (powerMeterTotal == null)
            {
                return NotFound();
            }

            powerMeterTotal.MapFromDTO(powerMeterTotalDTO);

            _context.Entry(powerMeterTotal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        
        // POST: PowerMeter/Total
        [HttpPost("Total")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterTotal>>> PostPowerMeterTotal(PowerMeterTotalDTO powerMeterTotalDTO)
        {
            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(powerMeterTotalDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {powerMeterTotalDTO.ThingId} not found";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            PowerMeterTotal powerMeterTotal = new PowerMeterTotal(powerMeterTotalDTO);

            _context.PowerMeterTotal.Add(powerMeterTotal);
            await _context.SaveChangesAsync();

            //await Notification(measure);

            return CreatedAtAction("GetPowerMeterTotal", new { id = powerMeterTotal.Id }, powerMeterTotal);
        }

        // DELETE: PowerMeter/Total/5
        [HttpDelete("Total/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterTotal>>> DeletePowerMeterTotal(long Id)
        {
            var powerMeterTotal = await _context.PowerMeterTotal.FindAsync(Id);
            if (powerMeterTotal == null)
            {
                return NotFound();
            }

            _context.PowerMeterTotal.Remove(powerMeterTotal);
            await _context.SaveChangesAsync();

            return Ok(new Response<PowerMeterTotal>(powerMeterTotal));
        }

        // GET: PowerMeter/Tariff
        [HttpGet("Tariff")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<PowerMeterTariff>>>> GetPowerMeterTariff([FromQuery] PowerMeterTariffPaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value;

            (var pagedData, var totalCount) = await filter.Process(_context.PowerMeterTariff, _context.Thing, new Guid(userId));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: PowerMeter/Tariff/5
        [HttpGet("Tariff/{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<PowerMeterTariff>>> GetPowerMeterTariff(long Id)
        {
            var measure = await _context.PowerMeterTariff.FindAsync(Id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(new Response<PowerMeterTariff>(measure));
        }

        // PUT: PowerMeter/Tariff/5
        [HttpPut("Tariff/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutPowerMeterTariff(long Id, PowerMeterTariffDTO powerMeterTariffDTO)
        {
            var powerMeterTariff = await _context.PowerMeterTariff.FindAsync(Id);

            if (powerMeterTariff == null)
            {
                return NotFound();
            }

            powerMeterTariff.MapFromDTO(powerMeterTariffDTO);

            _context.Entry(powerMeterTariff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: PowerMeter/Tariff
        [HttpPost("Tariff")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterTariff>>> PostPowerMeterTariff(PowerMeterTariffDTO powerMeterTariffDTO)
        {
            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(powerMeterTariffDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {powerMeterTariffDTO.ThingId} not found";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            PowerMeterTariff powerMeterTariff = new PowerMeterTariff(powerMeterTariffDTO);

            _context.PowerMeterTariff.Add(powerMeterTariff);
            await _context.SaveChangesAsync();

            //await Notification(measure);

            return CreatedAtAction("GetPowerMeterTariff", new { id = powerMeterTariff.Id }, powerMeterTariff);
        }

        // DELETE: PowerMeter/Tariff/5
        [HttpDelete("Tariff/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterTariff>>> DeletePowerMeterTariff(long Id)
        {
            var powerMeterTariff = await _context.PowerMeterTariff.FindAsync(Id);
            if (powerMeterTariff == null)
            {
                return NotFound();
            }

            _context.PowerMeterTariff.Remove(powerMeterTariff);
            await _context.SaveChangesAsync();

            return Ok(new Response<PowerMeterTariff>(powerMeterTariff));
        }

        // GET: PowerMeter/Instantaneous
        [HttpGet("Instantaneous")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<PowerMeterInstantaneous>>>> GetPowerMeterInstantaneous([FromQuery] PowerMeterInstantaneousPaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value;

            (var pagedData, var totalCount) = await filter.Process(_context.PowerMeterInstantaneous, _context.Thing, new Guid(userId));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: PowerMeter/Instantaneous/5
        [HttpGet("Instantaneous/{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<PowerMeterInstantaneous>>> GetPowerMeterInstantaneous(long Id)
        {
            var measure = await _context.PowerMeterInstantaneous.FindAsync(Id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(new Response<PowerMeterInstantaneous>(measure));
        }

        // PUT: PowerMeter/Instantaneous/5
        [HttpPut("Instantaneous/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutPowerMeterInstantaneous(long Id, PowerMeterInstantaneousDTO powerMeterInstantaneousDTO)
        {
            var powerMeterInstantaneous = await _context.PowerMeterInstantaneous.FindAsync(Id);

            if (powerMeterInstantaneous == null)
            {
                return NotFound();
            }

            powerMeterInstantaneous.MapFromDTO(powerMeterInstantaneousDTO);

            _context.Entry(powerMeterInstantaneous).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: PowerMeter/Instantaneous
        [HttpPost("Instantaneous")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterInstantaneous>>> PostPowerMeterInstantaneous(PowerMeterInstantaneousDTO powerMeterInstantaneousDTO)
        {
            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(powerMeterInstantaneousDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {powerMeterInstantaneousDTO.ThingId} not found";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            PowerMeterInstantaneous powerMeterInstantaneous = new PowerMeterInstantaneous(powerMeterInstantaneousDTO);

            _context.PowerMeterInstantaneous.Add(powerMeterInstantaneous);
            await _context.SaveChangesAsync();

            await Notification(powerMeterInstantaneous);

            return CreatedAtAction("GetPowerMeterInstantaneous", new { id = powerMeterInstantaneous.Id }, powerMeterInstantaneous);
        }

        // DELETE: PowerMeter/Instantaneous/5
        [HttpDelete("Instantaneous/{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<PowerMeterInstantaneous>>> DeletePowerMeterInstantaneous(long Id)
        {
            var powerMeterInstantaneous = await _context.PowerMeterInstantaneous.FindAsync(Id);
            if (powerMeterInstantaneous == null)
            {
                return NotFound();
            }

            _context.PowerMeterInstantaneous.Remove(powerMeterInstantaneous);
            await _context.SaveChangesAsync();

            return Ok(new Response<PowerMeterInstantaneous>(powerMeterInstantaneous));
        }

        public async Task Notification(PowerMeterInstantaneous powerMeterInstantaneous)
        {
            await _hubContext.Clients.All.SendAsync("PowerMeterInstantaneousNotification", powerMeterInstantaneous);
        }
    }
}
