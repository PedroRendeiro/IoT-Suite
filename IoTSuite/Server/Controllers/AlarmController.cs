using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTSuite.Shared;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using IoTSuite.Shared.Wrappers;
using IoTSuite.Server.Helpers;
using IoTSuite.Server.Services;
using Microsoft.AspNetCore.SignalR;
using IoTSuite.Server.Hubs;

namespace IoTSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AlarmsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalR> _hubContext;
        private readonly IUriService _uriService;

        public AlarmsController(ApplicationDbContext context, IHubContext<SignalR> hubContext, IUriService uriService = null)
        {
            _context = context;
            _hubContext = hubContext;
            _uriService = uriService;
        }

        // GET: api/Alarms
        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<PagedResponse<List<Alarm>>>> GetAlarm([FromQuery] AlarmPaginationFilter filter)
        {
            (var pagedData, var totalCount) = await filter.Process(_context.Alarm);

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: api/Alarms/5
        [HttpGet("{Id}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<Response<Alarm>>> GetAlarm(long Id)
        {
            var alarm = await _context.Alarm.FindAsync(Id);

            if (alarm == null)
            {
                return NotFound();
            }

            return Ok(new Response<Alarm>(alarm));
        }

        // PUT: api/Alarms
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutAlarm(long Id, AlarmDTO alarmDTO)
        {
            var alarm = await _context.Alarm.Where(user => user.Id.Equals(Id)).FirstAsync();

            if (alarm == null)
            {
                return NotFound();
            }

            alarm.MapFromDTO(alarmDTO);

            _context.Entry(alarm).State = EntityState.Modified;

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

        // POST: api/Alarms
        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Alarm>>> PostAlarm(AlarmDTO alarmDTO)
        {
            var alarm = new Alarm(alarmDTO);

            _context.Alarm.Add(alarm);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlarm", new { id = alarm.Id }, alarm);
        }

        // DELETE: api/Alarms/5
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<Response<Alarm>>> DeleteAlarm(long Id)
        {
            var alarm = await _context.Alarm.FindAsync(Id);
            if (alarm == null)
            {
                return NotFound();
            }

            _context.Alarm.Remove(alarm);
            await _context.SaveChangesAsync();

            return Ok(new Response<Alarm>(alarm));
        }

        // GET: api/Alarms/Users
        [HttpGet("Users")]
        [Authorize(Policy = "read", Roles = "Admin")]
        public async Task<ActionResult<PagedResponse<List<AlarmUser>>>> GetUser([FromQuery] AlarmUserPaginationFilter filter)
        {
            (var pagedData, var totalCount) = await filter.Process(_context.AlarmUser.Include(user => user.RFIDTags));

            var pagedReponse = PaginationHelper.CreatePagedReponse(
                await pagedData.ToListAsync(),
                filter,
                totalCount,
                _uriService,
                Request
            );

            return Ok(pagedReponse);
        }

        // GET: api/Alarms/Users/5
        [HttpGet("Users/{Id:int}")]
        [Authorize(Policy = "read", Roles = "Admin")]
        public async Task<ActionResult<Response<AlarmUser>>> GetUser(long Id)
        {
            var user = await _context.AlarmUser.Where(user => user.Id.Equals(Id)).Include(user => user.RFIDTags).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new Response<AlarmUser>(user));
        }

        // PUT: api/Alarms/Users/5
        [HttpPut("Users/{Id}")]
        [Authorize(Policy = "write", Roles = "Admin")]
        public async Task<IActionResult> PutUser(long Id, AlarmUserDTO alarmUserDTO)
        {
            var user = await _context.AlarmUser.Where(user => user.Id.Equals(Id)).FirstAsync();

            if (user == null)
            {
                return NotFound();
            }

            user.Username = alarmUserDTO.Username;

            user.RFIDTags = _context.RFIDTag.Where(tag => tag.AlarmUserId.Equals(Id)).ToList();

            user.RFIDTags.ToList().ForEach(tag =>
            {
                if (!alarmUserDTO.RFIDTags.Contains(tag.UID))
                {
                    user.RFIDTags.Remove(tag);
                }
            });

            alarmUserDTO.RFIDTags.ForEach(tag =>
            {
                if (!user.RFIDTags.Select(tag => tag.UID).Contains(tag))
                {
                    user.RFIDTags.Add(new RFIDTag
                    {
                        UID = tag
                    });
                }
            });

            _context.Entry(user).State = EntityState.Modified;

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

        // POST: api/Users
        [HttpPost("Users")]
        [Authorize(Policy = "write", Roles = "Admin")]
        public async Task<ActionResult<Response<AlarmUser>>> PostUser(AlarmUserDTO alarmUserDTO)
        {
            AlarmUser alarmUser = new AlarmUser
            {
                Username = alarmUserDTO.Username,
                RFIDTags = new List<RFIDTag>()
            };

            alarmUserDTO.RFIDTags.ForEach(tag =>
            {
                alarmUser.RFIDTags.Add(new RFIDTag
                {
                    UID = tag
                });
            });

            _context.AlarmUser.Add(alarmUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = alarmUser.Id }, alarmUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("Users/{Id}")]
        [Authorize(Policy = "write", Roles = "Admin")]
        public async Task<ActionResult<Response<AlarmUser>>> DeleteUser(long Id)
        {
            var user = await _context.AlarmUser.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            _context.AlarmUser.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new Response<AlarmUser>(user));
        }
    }
}
