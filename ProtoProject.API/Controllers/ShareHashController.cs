using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Data;
using ProtoProject.API.Models;
using System.Net.Mail;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShareHashController : ControllerBase
    {
        private readonly DataContext _context;

        public ShareHashController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShareHash>> GetHashById(int id)
        {
            var hash = _context.ShareHashes.FindAsync(id);

            if (hash == null)
            {
                return NotFound();
            }

            return Ok(hash);
        }

        [HttpGet("hashByCardId/{id}")]
        public async Task<ActionResult<ShareHash>> GetHashByCard(int id)
        {
            var hash = await _context.ShareHashes.Where(cg => cg.CardId == id).ToListAsync();

            if (hash == null)
            {
                return NotFound();
            }

            return Ok(hash[0]);
        }

        //[HttpPost("sendHashToUser/{userId}/{hash}")]
        //public async Task<ActionResult<ShareHash>> SendUser(int userId, string hash)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    var email = user.Email;

        //    try
        //    {
        //        var eAddress = new MailAddress(email);
        //    }
        //    catch
        //    {
        //        return BadRequest("email not valid");
        //    }


        //}
    }
}
