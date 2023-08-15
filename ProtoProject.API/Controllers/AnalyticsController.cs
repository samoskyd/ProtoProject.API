using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProtoProject.API.Data;
using ProtoProject.API.Models;

namespace ProtoProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly DataContext _context;
        public AnalyticsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("comlpetedAndInProgress/{userId}")]
        public ActionResult<Dictionary<string, int>> AnalyzeCompletedAndInProgressTasks(int userId)
        {
            var analytics = new Dictionary<string, int>();

            var userCards = _context.Cards.Where(c => c.UserId == userId).ToList();

            int completedTasks = userCards.Count(c => c.Completion == Completion.Done);

            DateTime currentDate = DateTime.Now;
            int inProgressTasks = userCards.Count(c => c.Completion != Completion.Done && c.Deadline.HasValue && c.Deadline > currentDate);

            analytics.Add("CompletedTasks", completedTasks);
            analytics.Add("InProgressTasks", inProgressTasks);

            return Ok(analytics);
        }

        [HttpGet("priority/{userId}")]
        public ActionResult<Dictionary<string, int>> AnalyzeTasksByPriority(int userId)
        {
            var analytics = new Dictionary<string, int>();

            var userCards = _context.Cards.Where(c => c.UserId == userId).ToList();

            int lowPriorityTasks = userCards.Count(c => c.Priority == Priority.Low);
            int averagePriorityTasks = userCards.Count(c => c.Priority == Priority.Average);
            int highPriorityTasks = userCards.Count(c => c.Priority == Priority.High);

            analytics.Add("LowPriorityTasks", lowPriorityTasks);
            analytics.Add("AveragePriorityTasks", averagePriorityTasks);
            analytics.Add("HighPriorityTasks", highPriorityTasks);

            return Ok(analytics);
        }

        [HttpGet("byTag/{userId}")]
        public ActionResult<Dictionary<string, int>> AnalyzeTasksByTag(int userId)
        {
            var analytics = new Dictionary<string, int>();

            var userCards = _context.Cards.Where(c => c.UserId == userId).ToList();

            var tasksByTag = userCards.GroupBy(c => c.Tag)
                                      .Select(g => new { Tag = g.Key, Count = g.Count() });

            foreach (var task in tasksByTag)
            {
                analytics.Add($"Tasks With Tag {task.Tag}:", task.Count);
            }

            return Ok(analytics);
        }

        [HttpGet("overdue/{userId}")]
        public ActionResult<Dictionary<string, int>> AnalyzeOverdueTasks(int userId)
        {
            var analytics = new Dictionary<string, int>();
            var userCards = _context.Cards.Where(c => c.UserId == userId).ToList();

            DateTime currentDate = DateTime.Now;
            int overdueTasks = userCards.Count(c => c.Completion != Completion.Done && c.Deadline.HasValue && c.Deadline < currentDate);

            analytics.Add("OverdueTasks", overdueTasks);

            return Ok(analytics);
        }
    }
}
