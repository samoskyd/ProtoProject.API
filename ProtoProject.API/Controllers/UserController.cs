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
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(c => c.Cards)
                .Include(c => c.UserGroups)
                .Include(c => c.Friendships)
                .Include(c => c.CardGroups)
                .Include(c => c.CardSequences)
                .FirstOrDefaultAsync(c => c.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("usersByName/{name}")]
        public async Task<ActionResult<List<User>>> UsersByName(string name)
        {
            var users = await _context.Users.Where(cg => cg.Name == name).ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpGet("usersByMail/{mail}")]
        public async Task<ActionResult<List<User>>> UsersByMail(string mail)
        {
            var users = await _context.Users.Where(cg => cg.Email == mail).ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users[0]);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (user == null) return BadRequest();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }


        [HttpPut]
        public async Task<ActionResult<User>> UpdateUser(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.UserId);
            if (dbUser == null)
                return BadRequest("Task not found");

            dbUser.Name = user.Name;
            dbUser.Email = user.Email;
            dbUser.Password = user.Password;


            await _context.SaveChangesAsync();
            return Ok(dbUser);
        }
    }
}
