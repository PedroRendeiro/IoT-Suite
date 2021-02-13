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
using IoTSuite.Server.Helpers;
using IoTSuite.Server.Services;
using IoTSuite.Server.Filters;
using IoTSuite.Shared.Filters;
using IoTSuite.Shared.Wrappers;

namespace IoTSuite.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class MeasuresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalR> _hubContext;
        private readonly IUriService _uriService;

        public MeasuresController(ApplicationDbContext context, IHubContext<SignalR> hubContext, IUriService uriService = null)
        {
            _context = context;
            _hubContext = hubContext;
            _uriService = uriService;
        }

        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<Measure>>>> GetMeasure([FromQuery] MeasurePaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value; 
            
            (var pagedData, var totalCount) = await filter.Process(_context.Measure, _context.Thing, new Guid(userId));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(), 
                filter,
                totalCount,
                _uriService,
                Request
            );
            
            return Ok(pagedReponse);
        }

        // GET: Measures/5
        [HttpGet("{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<Measure>>> GetMeasure(long Id)
        {
            var measure = await _context.Measure.FindAsync(Id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(new Response<Measure>(measure));
        }

        // PUT: Measures/5
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutMeasure(long Id, MeasureDTO measureDTO)
        {
            var measure = await _context.Measure.FindAsync(Id);

            if (measure == null)
            {
                return NotFound();
            }

            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(measureDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {measureDTO.ThingId} not found";
            }
            else if (!thing.Features.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList().Contains(measureDTO.Type.ToString().Trim()))
            {
                message = $"Thing with Id {measureDTO.ThingId} does not have the specified feature {measureDTO.Type}";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            measure.MapFromDTO(measureDTO);

            _context.Entry(measure).State = EntityState.Modified;

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

        // POST: Measures
        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Measure>>> PostMeasure(MeasureDTO measureDTO)
        {
            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(measureDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {measureDTO.ThingId} not found";
            }
            else if (!thing.Features.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList().Contains(measureDTO.Type.ToString().Trim()))
            {
                message = $"Thing with Id {measureDTO.ThingId} does not have the specified feature {measureDTO.Type}";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            Measure measure = new Measure(measureDTO);

            _context.Measure.Add(measure);
            await _context.SaveChangesAsync();

            await Notification(measure);

            return CreatedAtAction("GetMeasure", new { id = measure.Id }, measure);
        }

        // DELETE: measuress/5
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Measure>>> DeleteMeasure(long Id)
        {
            var measure = await _context.Measure.FindAsync(Id);
            if (measure == null)
            {
                return NotFound();
            }

            _context.Measure.Remove(measure);
            await _context.SaveChangesAsync();

            return Ok(new Response<Measure>(measure));
        }

        public async Task Notification(Measure measure)
        {
            await _hubContext.Clients.All.SendAsync("MeasureNotification", measure);
        }

    }
}
