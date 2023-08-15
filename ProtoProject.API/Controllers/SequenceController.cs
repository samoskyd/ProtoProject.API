using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Versioning;
using ProtoProject.API.Data;
using ProtoProject.API.Models;
using System.Linq;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SequenceController : ControllerBase
    {
        private readonly DataContext _context;

        public SequenceController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("sequences/{userId}")]
        public async Task<ActionResult<List<CardSequence>>> GetLongSequences(int userId)
        {
            var userCards = await _context.Cards
                .Include(c => c.PreviousCard)
                .Include(c => c.NextCard)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var sequenceStartCards = userCards.Where(c => c.PreviousCardId == null && c.NextCardId != null).ToList();

            var longSequences = sequenceStartCards.Select(startCard =>
            {
                var currentCard = startCard;
                var sequenceLength = 1;
                var cardsInSequence = new List<Card> { currentCard };

                while (currentCard.NextCard != null)
                {
                    currentCard = currentCard.NextCard;
                    sequenceLength++;
                    cardsInSequence.Add(currentCard);
                }

                var cq = new CardSequence
                {
                    CardSequenceName = "Sequence Name",
                    CardSequenceDescription = "Sequence Description",
                    StartCardId = startCard.CardId,
                    StartCard = startCard,
                    UserId = userId,
                    User = startCard.User,
                    Cards = cardsInSequence
                };

                foreach (var c in cq.Cards)
                {
                    c.CardSequenceId = cq.CardSequenceId;
                }

                return cq;
            })
            .Where(sequence => sequence.Cards.Count > 1)
            .ToList();

            var userSequences = await _context.CardSequences.Where(c => c.UserId == userId).ToListAsync();
            if (userSequences != null) 
            {
                foreach (var cs in userSequences)
                {
                    _context.Remove(cs);
                }
            }
            foreach (var cs in longSequences)
            {
                _context.Add(cs);
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return BadRequest("User not found");
            user.CardSequences = longSequences;

            await _context.SaveChangesAsync();

            return Ok(longSequences);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardSequence>> GetSequence(int id)
        {
            var s = await _context.CardSequences.FindAsync(id);

            if (s == null)
            {
                return NotFound();
            }

            return Ok(s);
        }

        [HttpPut]
        public async Task<ActionResult<CardSequence>> UpdateSequence(CardSequence s)
        {
            var dbs = await _context.CardSequences.FindAsync(s.CardSequenceId);
            if (dbs == null)
                return BadRequest("Task not found");

            dbs.CardSequenceName = s.CardSequenceName;
            dbs.CardSequenceDescription = s.CardSequenceDescription;

            await _context.SaveChangesAsync();
            return Ok(dbs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSequence(int id)
        {
            var sequence = await _context.CardSequences
                .Include(s => s.Cards)
                .FirstOrDefaultAsync(s => s.CardSequenceId == id);

            if (sequence == null)
                return BadRequest("Sequence not found");

            // Remove the sequence reference from associated cards
            foreach (var card in sequence.Cards)
            {
                card.CardSequenceId = null;
            }

            _context.CardSequences.Remove(sequence);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
