using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTSuite.Shared;
using IoTSuite.Server.Services;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;

namespace IoTSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = (UserService)userService;
        }

        // GET: api/User
        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<IEnumerable<BasicAuthUser>> GetUser()
        {
            return await _userService.GetAll();
        }

        // GET: api/User/5
        [HttpGet("{Id:int}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<BasicAuthUser>> GetUser(long Id)
        {
            var user = await _userService.GetAll(Id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/User/Username
        [HttpGet("{Username}")]
        [Authorize(Policy = "read")]
        public async Task<ActionResult<BasicAuthUser>> GetUser([FromRoute]string Username)
        {
            var user = await _userService.GetAll(Username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        [HttpPut("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<IActionResult> PutUser(long Id, BasicAuthUserDTO userDTO)
        {

            var user = await _userService.UpdateUser(Id, userDTO);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<BasicAuthUser>> PostUser(BasicAuthUserDTO userDTO)
        {
            var user = await _userService.CreateUser(userDTO);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{Id}")]
        [Authorize(Policy = "write")]
        public async Task<ActionResult<BasicAuthUser>> DeleteUser(long Id)
        {
            var user = await _userService.DeleteUser(Id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
    }
}
