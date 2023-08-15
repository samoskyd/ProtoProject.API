using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoProject.API.Data;
using ProtoProject.API.Models;
using System.Text;
using System.Security.Cryptography;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardController : ControllerBase
    {
        private readonly DataContext _context;

        public CardController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            var card = await _context.Cards
                .Include(c => c.ShareHash)
                .Include(c => c.CardGroup)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CardId == id);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpGet("cardByHashId/{id}")]
        public async Task<ActionResult<Card>> GetCardByHash(int id)
        {
            var card = await _context.Cards.Where(cg => cg.ShareHash.ShareHashId == id).ToListAsync();

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card[0]);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<Card>> GetCardsByUser(int id)
        {
            var cards = await _context.Cards.Where(c => c.UserId == id).ToListAsync();

            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        [HttpGet("tag/{userId}/{tag}")]
        public async Task<ActionResult<Card>> GetCardsByTag(int userId, string tag)
        {
            var userCardsAndTags = _context.Cards.Where(c => c.UserId == userId && c.Tag == tag).ToList();

            if (userCardsAndTags == null)
            {
                return NotFound();
            }

            return Ok(userCardsAndTags);
        }

        [HttpGet("date/{id}/startInterval/{startDate}/{endDate}")]
        public async Task<ActionResult<List<Card>>> StartInterval (int id, DateTime startDate, DateTime endDate)
        {
            var cards = await _context.Cards
                .Where(c => c.UserId == id && c.StartDate >= startDate && c.StartDate <= endDate)
                .ToListAsync();

            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        [HttpGet("date/{id}/endInterval/{startDate}/{endDate}")]
        public async Task<ActionResult<List<Card>>> DeadlineInterval(int id, DateTime startDate, DateTime endDate)
        {
            var cards = await _context.Cards
                .Where(c => c.UserId == id && c.Deadline >= startDate && c.Deadline <= endDate)
                .ToListAsync();

            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        [HttpGet("date/{id}/wholeInterval/{startDate}/{endDate}")]
        public async Task<ActionResult<List<Card>>> WholeInterval (int id, DateTime startDate, DateTime endDate)
        {
            var cards = await _context.Cards
                .Where(c => c.UserId == id && c.StartDate >= startDate && c.Deadline <= endDate)
                .ToListAsync();

            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        [HttpGet("group/{id}")]
        public async Task<ActionResult<Card>> GetCardsByGroup(int id)
        {
            var cards = await _context.Cards.Where(c => c.CardGroupId == id).ToListAsync();

            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        [HttpPost]
        public async Task<ActionResult<Card>> CreateCard(Card card)
        {
            try
            {
                if (card == null)
                {
                    return BadRequest("Invalid card data.");
                }

                if (card.StartDate > card.Deadline)
                {
                    return BadRequest("Invalid card dates.");
                }

                var user = await _context.Users.FindAsync(card.UserId);
                if (user == null)
                {
                    return BadRequest("Card user not found");
                }
                card.User = user;

                if (card.NextCardId != null) 
                {
                    var nc = await _context.Cards.FindAsync(card.NextCardId);
                    if (nc != null)
                    {
                        card.NextCard = nc;
                        nc.PreviousCard = card;
                        nc.PreviousCardId = card.CardId;
                    }
                    else return BadRequest("Invalid Next Card Data");
                }

                if (card.PreviousCardId != null) 
                {
                    var pc = await _context.Cards.FindAsync(card.PreviousCardId);
                    if (pc != null) 
                    {
                        card.PreviousCard = pc;
                        pc.NextCard = card;
                        pc.NextCardId = card.CardId;
                    }
                    else return BadRequest("Invalid Previous Card Data");
                }

                if (card.CardGroupId != null)
                {
                    var cg = await _context.CardGroups.FindAsync(card.CardGroupId);
                    if (cg != null)
                    {
                        card.CardGroup = cg;
                        cg.Cards?.Add(card);
                    }
                    else return BadRequest("Invalid Card Group Data");
                }

                var linkFolder = new LinkFolder { CardId = card.CardId, Card = card, ContainerName = "links" };
                var imageFolder = new ImageFolder { CardId = card.CardId, Card = card, ContainerName = "images" };
                var docFolder = new DocFolder { CardId = card.CardId, Card = card, ContainerName = "docs" };
                card.LinkFolder = linkFolder;
                card.ImageFolder = imageFolder;
                card.DocFolder = docFolder;

                _context.LinkFolders.Add(linkFolder);
                _context.ImageFolders.Add(imageFolder);
                _context.DocFolders.Add(docFolder);

                _context.Cards.Add(card);
                await _context.SaveChangesAsync();

                var shareHash = await CreateShareHash(card.CardId);
                card.ShareHash = shareHash;
                card.LinkFolderId = card.LinkFolder.LinkFolderId;
                card.ImageFolderId = card.ImageFolder.ImageFolderId;
                card.DocFolderId = card.DocFolder.DocFolderId;

                await _context.SaveChangesAsync();

                return Ok(card);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the card: {ex.Message}");
            }
        }


        [HttpPost("createCardByHash/{hash}/{userId}")]
        public async Task<IActionResult> CreateCardByHash(string hash, int userId)
        {
            var shareHash = await _context.ShareHashes.FirstAsync(h => h.Hash == hash);

            if (shareHash == null)
            {
                return NotFound();
            }

            var existingCard = await _context.Cards.FindAsync(shareHash.CardId);

            if (existingCard == null)
            {
                return NotFound();
            }

            var newCard = new Card
            {
                Name = existingCard.Name,
                StartDate = existingCard.StartDate,
                Deadline = existingCard.Deadline,
                Info = existingCard.Info,
                UserId = userId,
                Tag = existingCard.Tag,
                Grade = existingCard.Grade,
                Completion = existingCard.Completion,
                Priority = existingCard.Priority
            };
            newCard.ShareHash = await CreateShareHash(newCard.CardId);

            var user = await _context.Users.FindAsync(newCard.UserId);
            if (user == null)
            {
                return BadRequest("Card user not found");
            }
            newCard.User = user;

            var linkFolder = new LinkFolder { CardId = newCard.CardId, Card = newCard, ContainerName = "links" };
            var imageFolder = new ImageFolder { CardId = newCard.CardId, Card = newCard, ContainerName = "images" };
            var docFolder = new DocFolder { CardId = newCard.CardId, Card = newCard, ContainerName = "docs" };

            newCard.LinkFolder = linkFolder;
            newCard.ImageFolder = imageFolder;
            newCard.DocFolder = docFolder;

            _context.LinkFolders.Add(linkFolder);
            _context.ImageFolders.Add(imageFolder);
            _context.DocFolders.Add(docFolder);

            _context.Cards.Add(newCard);
            await _context.SaveChangesAsync();

            return Ok(newCard);
        }

        private async Task<ShareHash> CreateShareHash(int id)
        {
            string hash = "";

            using (var sha256 = SHA256.Create())
            {
                int maxAttempts = 100;
                int attempts = 0;

                while (attempts < maxAttempts)
                {
                    hash = GetHashString(sha256, id.ToString() + DateTime.UtcNow.Ticks.ToString());
                    if (!await _context.ShareHashes.AnyAsync(c => c.Hash == hash))
                        break;

                    attempts++;
                }
                if (attempts >= maxAttempts)
                {
                    throw new Exception("Unable to generate a unique hash for the card.");
                }
            }

            var card = await _context.Cards.FindAsync(id);
            ShareHash res = new ShareHash
            {
                CardId = id,
                Hash = hash,
                Card = card
            };

            _context.ShareHashes.Add(res);
            await _context.SaveChangesAsync();
            return res;
        }

        private string GetHashString(SHA256 sha256, string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            StringBuilder sbuilder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sbuilder.Append(hashBytes[i].ToString("x2"));
            }

            return sbuilder.ToString();
        }

        [HttpPut]
        public async Task<ActionResult<Card>> UpdateCard(Card card)
        {
            var dbCard = await _context.Cards.FindAsync(card.CardId);
            if (dbCard == null)
                return BadRequest("Task not found");

            dbCard.Name = card.Name;
            dbCard.StartDate = card.StartDate;
            dbCard.Deadline = card.Deadline;
            dbCard.Info = card.Info;
            dbCard.CardGroupId = card.CardGroupId;
            dbCard.Tag = card.Tag;
            dbCard.PreviousCardId = card.PreviousCardId;
            dbCard.NextCardId = card.NextCardId;

            if (dbCard.NextCardId != null)
            {
                var nc = await _context.Cards.FindAsync(dbCard.NextCardId);
                if (nc != null)
                {
                    dbCard.NextCard = nc;
                    nc.PreviousCard = dbCard;
                    nc.PreviousCardId = dbCard.CardId;
                }
                else return BadRequest("Invalid Next Card Data");
            }

            if (dbCard.PreviousCardId != null)
            {
                var pc = await _context.Cards.FindAsync(dbCard.PreviousCardId);
                if (pc != null)
                {
                    dbCard.PreviousCard = pc;
                    pc.NextCard = dbCard;
                    pc.NextCardId = dbCard.CardId;
                }
                else return BadRequest("Invalid Previous Card Data");
            }

            if (dbCard.CardGroupId != null)
            {
                var cg = await _context.CardGroups.FindAsync(dbCard.CardGroupId);
                if (cg != null)
                {
                    dbCard.CardGroup = cg;
                    cg.Cards?.Add(dbCard);
                }
                else return BadRequest("Invalid Card Group Data");
            }

            await _context.SaveChangesAsync();
            return Ok(dbCard);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var dbCard = await _context.Cards
                .Include(c => c.CardGroup)
                .Include(c => c.User)
                .Include(c => c.PreviousCard)
                .Include(c => c.NextCard)
                .Include(c => c.ShareHash)
                .Include(c => c.DocFolder)
                .Include(c => c.LinkFolder)
                .Include(c => c.ImageFolder)
                .FirstOrDefaultAsync(c => c.CardId == id);

            if (dbCard == null)
                return BadRequest("Card not found");

            if (dbCard.CardGroup != null)
                dbCard.CardGroup.Cards.Remove(dbCard);

            if (dbCard.User != null)
                dbCard.User.Cards.Remove(dbCard);

            if (dbCard.PreviousCard != null)
                dbCard.PreviousCard.NextCard = null;

            if (dbCard.NextCard != null)
                dbCard.NextCard.PreviousCard = null;

            if (dbCard.ShareHash != null)
                _context.ShareHashes.Remove(dbCard.ShareHash);

            if (dbCard.DocFolder != null)
                _context.DocFolders.Remove(dbCard.DocFolder);

            if (dbCard.LinkFolder != null)
                _context.LinkFolders.Remove(dbCard.LinkFolder);

            if (dbCard.ImageFolder != null)
                _context.ImageFolders.Remove(dbCard.ImageFolder);

            if (dbCard.CardSequenceId != null)
            {
                if (dbCard.NextCard != null && dbCard.NextCardId != null)
                {
                    var seq = await _context.CardSequences.FindAsync(dbCard.CardSequenceId);
                    seq.StartCard = dbCard.NextCard;
                    seq.StartCardId = (int)dbCard.NextCardId;
                }
            }

            _context.Cards.Remove(dbCard);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}