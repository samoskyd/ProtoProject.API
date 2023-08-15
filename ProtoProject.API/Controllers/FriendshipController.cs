using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Data;
using ProtoProject.API.Models;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendshipController : ControllerBase
    {
        private readonly DataContext _context;

        public FriendshipController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("friendshipByUser/{id}")]
        public async Task<ActionResult<List<Friendship>>> GetFriendship(int id)
        {
            var friendships = await _context.Friendships
                .Include(cg => cg.FUser)
                .Include(cg => cg.SUser)
                .Where(cg => cg.FUserId == id || cg.SUserId == id).ToListAsync();

            if (friendships == null)
            {
                return NotFound();
            }

            return Ok(friendships);
        }
        
        [HttpPost]
        public async Task<ActionResult<Friendship>> CreateFriendship(Friendship friendship)
        {
            if (friendship == null) return BadRequest();

            var fuser = await _context.Users.FindAsync(friendship.FUserId);
            friendship.FUser = fuser;
            var suser = await _context.Users.FindAsync(friendship.SUserId);
            friendship.SUser = suser;

            if (fuser == null || suser == null) 
            {
                return BadRequest("one or two users are invalid");
            }
            
            fuser?.Friendships?.Add(friendship);
            suser?.Friendships?.Add(friendship);

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok(friendship);
        }

        [HttpPost("users/{id1}/{id2}")]
        public async Task<ActionResult<Friendship>> CreateFriendshipByUsers(int id1, int id2)
        {
            var fUser = await _context.Users.FindAsync(id1);
            var sUser = await _context.Users.FindAsync(id2);

            if (fUser == null || sUser == null)
            {
                return NotFound("One or both users not found");
            }

            var friendship = new Friendship
            {
                FUserId = id1,
                FUser = fUser,
                SUserId = id2,
                SUser = sUser
            };

            fUser?.Friendships?.Add(friendship);
            sUser?.Friendships?.Add(friendship);

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok(friendship);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendship(int id)
        {
            var dbGroup = await _context.Friendships.FindAsync(id);
            if (dbGroup == null)
                return BadRequest("Task not found");

            _context.Friendships.Remove(dbGroup);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("users/{id1}/{id2}")]
        public async Task<IActionResult> DeleteFriendshipByUsers(int id1, int id2)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.FUserId == id1 && f.SUserId == id2) ||
                (f.FUserId == id2 && f.SUserId == id1));

            if (friendship == null)
            {
                return BadRequest("Friendship not found");
            }

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
