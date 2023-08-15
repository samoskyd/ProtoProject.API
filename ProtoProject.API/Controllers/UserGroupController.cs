using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Data;
using ProtoProject.API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserGroupController : ControllerBase
    {
        private readonly DataContext _context;

        public UserGroupController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("groupByGroupId/{id}")]
        public async Task<ActionResult<UserGroup>> GetUserGroup(int id)
        {
            var userGroup = await _context.UserGroups
                .Include(ug => ug.Users)
                .FirstOrDefaultAsync(ug => ug.UserGroupId == id);

            if (userGroup == null)
            {
                return NotFound();
            }

            return Ok(userGroup);
        }

        [HttpGet("groupsByUserId/{id}")]
        public async Task<ActionResult<List<UserGroup>>> GetUserGroupsByUser(int id)
        {
            var userGroups = await _context.UserGroups
                .Where(ug => ug.Users.Any(u => u.UserId == id))
                .ToListAsync();

            if (userGroups == null)
            {
                return NotFound();
            }

            return Ok(userGroups);
        }

        [HttpPost]
        public async Task<ActionResult<UserGroup>> CreateUserGroup(UserGroup userGroup)
        {
            if (userGroup == null) return BadRequest();

            var admin = await _context.Users.FindAsync(userGroup.AdminId);
            if (admin == null)
                return BadRequest("Task not found");
            userGroup.Admin = admin;
            userGroup.Users?.Add(admin);
            admin.UserGroups?.Add(userGroup);

            _context.UserGroups.Add(userGroup);
            await _context.SaveChangesAsync();

            return Ok(userGroup);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserGroup>> UpdateUserGroup(UserGroup userGroup, int id)
        {
            if (userGroup.AdminId != id)
                return BadRequest("unsufficient rights");

            var dbGroup = await _context.UserGroups.FindAsync(userGroup.UserGroupId);
            if (dbGroup == null)
                return BadRequest("Task not found");

            dbGroup.Name = userGroup.Name;
            dbGroup.Description = userGroup.Description;

            await _context.SaveChangesAsync();
            return Ok(dbGroup);
        }

        [HttpPut("userToGroup/{userId}/{groupId}")]
        public async Task<ActionResult<UserGroup>> UpdateUserGroup(int userId, int groupId)
        {
            var dbGroup = await _context.UserGroups.FindAsync(groupId);
            if (dbGroup == null)
                return BadRequest("Task not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("Task not found");

            dbGroup.Users.Add(user);
            user.UserGroups.Add(dbGroup);

            await _context.SaveChangesAsync();
            return Ok(dbGroup);
        }

        [HttpDelete("{id}/{userId}")]
        public async Task<IActionResult> DeleteUserGroup(int id, int userId)
        {
            var dbGroup = await _context.UserGroups.FindAsync(id);
            if (dbGroup == null)
                return BadRequest("Task not found");

            if (dbGroup.AdminId != userId)
                return BadRequest("unsufficient rights");

            _context.UserGroups.Remove(dbGroup);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
