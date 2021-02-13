using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTSuite.Shared;
using IoTSuite.Server.Services;
using IoTSuite.Server.Helpers;
using IoTSuite.Shared.Wrappers;

namespace IoTSuite.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ThingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUriService _uriService;
        public static List<Thing> GetThings { get; private set; }
        public static List<Policy> GetPolicies { get; private set; }

        public static void UpdateThingsAndPolicies(ApplicationDbContext context)
        {
            GetThings = context.Thing.AsNoTracking().ToList();
            GetPolicies = context.Policy.AsNoTracking().ToList();
        }

        public ThingsController(ApplicationDbContext context, IUriService uriService = null)
        {
            _context = context;
            _uriService = uriService;
        }

        // GET: Things
        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<Thing>>>> GetThing([FromQuery] ThingPaginationFilter filter)
        {
            var userId = HttpContext.User
                .Claims
                .Where(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))
                .First()
                .Value;

            if (filter.Owner.HasValue)
            {
                if (filter.Owner.Value.ToString() != userId & !HttpContext.User.IsInRole("Admin"))
                {
                    return Forbid();
                }
            }

            var things = Thing.Filter(_context.Thing, new Guid(userId));

            (var pagedData, var totalCount) = await filter.Process(things);

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: Things/5
        [HttpGet("{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<Thing>>> GetThing(Guid Id)
        {
            var thing = await _context.Thing.AsNoTracking().Where(thing => thing.ThingId.Equals(Id)).Include(thing => thing.Users).FirstOrDefaultAsync();

            if (thing == null)
            {
                return NotFound();
            }

            return Ok(new Response<Thing>(thing));
        }

        // PUT: Things/5
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutThing(Guid Id, ThingDTO thingDTO)
        {
            var thing = await _context.Thing.Include(thing => thing.Users).Where(thing => thing.ThingId.Equals(Id)).FirstOrDefaultAsync();

            if (thing == null)
            {
                return NotFound();
            }

            thing.MapFromDTO(thingDTO);

            _context.Entry(thing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            UpdateThingsAndPolicies(_context);

            return NoContent();
        }

        // POST: Things
        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Thing>>> PostThing(ThingDTO thingDTO)
        {
            Thing thing = new Thing
            {
                Owner = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Users = new List<Policy>(),
                Type = thingDTO.Type,
                Features = thingDTO.Features
            };

            thingDTO.Users.ForEach(user =>
            {
                thing.Users.Add(new Policy
                {
                    User = user
                });
            });

            _context.Thing.Add(thing);
            await _context.SaveChangesAsync();

            UpdateThingsAndPolicies(_context);

            return CreatedAtAction("GetThing", new { Id = thing.ThingId }, thing);
        }

        // DELETE: Things/5
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Thing>>> DeleteThing(Guid Id)
        {
            var thing = await _context.Thing.FindAsync(Id);
            if (thing == null)
            {
                return NotFound();
            }

            _context.Thing.Remove(thing);
            await _context.SaveChangesAsync();

            UpdateThingsAndPolicies(_context);

            return Ok(new Response<Thing>(thing));
        }
    }
}
