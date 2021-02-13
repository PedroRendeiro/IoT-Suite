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
using IoTSuite.Shared.Wrappers;

namespace IoTSuite.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class PositionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalR> _hubContext;
        private readonly IUriService _uriService;

        public PositionsController(ApplicationDbContext context, IHubContext<SignalR> hubContext, IUriService uriService = null)
        {
            _context = context;
            _hubContext = hubContext;
            _uriService = uriService;
        }

        // GET: Positions
        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<Position>>>> GetPosition([FromQuery] PositionPaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value;

            (var pagedData, var totalCount) = await filter.Process(_context.Position, _context.Thing, new Guid(userId));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: Positions/5
        [HttpGet("{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<Position>>> GetPosition(long Id)
        {
            var position = await _context.Position.FindAsync(Id);

            if (position == null)
            {
                return NotFound();
            }

            return Ok(new Response<Position>(position));
        }

        // PUT: Positions/5
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutPosition(long Id, PositionDTO positionDTO)
        {
            var position = await _context.Position.FindAsync(Id);

            if (position == null)
            {
                return NotFound();
            }

            position.MapFromDTO(positionDTO);

            _context.Entry(position).State = EntityState.Modified;

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

        // POST: Positions
        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Position>>> PostPosition(PositionDTO positionDTO)
        {
            var thing = _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(positionDTO.ThingId)).FirstOrDefault();

            string message = string.Empty;
            if (thing == null)
            {
                message = $"Thing with Id {positionDTO.ThingId} not found";
            }
            else if (!thing.Features.HasFlag(FeatureType.Tracker))
            {
                message = $"Thing with Id {positionDTO.ThingId} does not have the specified feature {FeatureType.Tracker}";
            }

            if (message != string.Empty)
            {
                Debug.WriteLine(message);
                return BadRequest(new Response<Exception>(
                    HttpStatusCode.BadRequest,
                    message
                ));
            }

            Position position = new Position
            {
                Date = DateTime.Now,
                ThingId = positionDTO.ThingId,
                Latitude = positionDTO.Latitude,
                Longitude = positionDTO.Longitude,
                Velocity = positionDTO.Velocity,
                Course = positionDTO.Course
            };

            _context.Position.Add(position);
            await _context.SaveChangesAsync();

            await Notification(position);

            return CreatedAtAction("GetPosition", new { id = position.Id }, position);
        }

        // DELETE: Positions/5
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Position>>> DeletePosition(long Id)
        {
            var position = await _context.Position.FindAsync(Id);
            if (position == null)
            {
                return NotFound();
            }

            _context.Position.Remove(position);
            await _context.SaveChangesAsync();

            return Ok(new Response<Position>(position));
        }

        public async Task Notification(Position position)
        {
            await _hubContext.Clients.All.SendAsync("PositionNotification", position);
        }
    }
}
