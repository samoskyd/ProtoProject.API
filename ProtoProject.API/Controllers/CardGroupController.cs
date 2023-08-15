using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using ProtoProject.API.Data;
using ProtoProject.API.Models;

namespace ProtoProject.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardGroupController : ControllerBase
    {
        private readonly DataContext _context;

        public CardGroupController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardGroup>> GetCardGroup(int id)
        {
            var cardGroup = await _context.CardGroups
                .Include(cg => cg.Cards)
                .Include(cg => cg.User)
                .FirstOrDefaultAsync(cg => cg.CardGroupId == id);

            if (cardGroup == null)
            {
                return NotFound();
            }

            return Ok(cardGroup);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<CardGroup>> GetCardGroupsByUser(int id)
        {
            var cardGroups = await _context.CardGroups.Where(cg => cg.UserId == id).ToListAsync();

            if (cardGroups == null)
            {
                return NotFound();
            }

            return Ok(cardGroups);
        }

        [HttpPost]
        public async Task<ActionResult<CardGroup>> CreateCardGroup(CardGroup cardGroup)
        {
            if (cardGroup == null) return BadRequest();

            var user = await _context.Users.FindAsync(cardGroup.UserId);
            if (user == null) 
            {
                return BadRequest("user not found");
            }
            cardGroup.User = user;
            user.CardGroups?.Add(cardGroup);

            _context.CardGroups.Add(cardGroup);
            await _context.SaveChangesAsync();

            return Ok(cardGroup);
        }

        [HttpPut]
        public async Task<ActionResult<CardGroup>> UpdateCardGroup(CardGroup cardGroup)
        {
            var dbCardGroup = await _context.CardGroups.FindAsync(cardGroup.CardGroupId);
            if (dbCardGroup == null)
                return BadRequest("Task not found");

            dbCardGroup.Name = cardGroup.Name;
            dbCardGroup.Description = cardGroup.Description;

            await _context.SaveChangesAsync();
            return Ok(dbCardGroup);
        }

        [HttpPut("ctg/{cardId}/{groupId}")]
        public async Task<ActionResult<CardGroup>> CardToGroup(int cardId, int groupId)
        {
            var dbCardGroup = await _context.CardGroups.FindAsync(groupId);
            if (dbCardGroup == null)
                return BadRequest("Group not found");

            var dbCard = await _context.Cards.FindAsync(cardId);
            if (dbCard == null)
                return BadRequest("Task not found");

            dbCardGroup.Cards?.Add(dbCard);
            dbCard.CardGroup = dbCardGroup;
            dbCard.CardGroupId = groupId;

            await _context.SaveChangesAsync();
            return Ok(dbCardGroup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardGroup(int id)
        {
            var dbCardGroup = await _context.CardGroups.FindAsync(id);
            if (dbCardGroup == null)
                return BadRequest("Group not found");

            var cgCards = await _context.Cards.Where(c => c.CardGroupId == dbCardGroup.CardGroupId).ToListAsync();
            foreach (var c in cgCards)
            {
                c.CardGroupId = null;
            }

            var user = await _context.Users.FindAsync(dbCardGroup.UserId);
            user?.CardGroups?.Remove(dbCardGroup);

            _context.CardGroups.Remove(dbCardGroup);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
