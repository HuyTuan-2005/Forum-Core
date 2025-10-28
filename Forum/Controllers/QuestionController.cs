using System.Security.Claims;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers
{
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Questions.Include(x => x.Tags).Include(x => x.Answer).ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue)
            {
                var question = await _context.Questions
                    .Include(q => q.User).ThenInclude(u => u.Profile)
                    .Include(q => q.Tags)
                    .Include(q => q.Answer).ThenInclude(a => a.User).ThenInclude(p => p.Profile) 
                    .FirstOrDefaultAsync(q => q.QuestionId == id.Value);
                
                // ← QUAN TRỌNG: Check null trước khi return
                if (question == null)
                    return NotFound();  // Return 404 nếu không tìm thấy
                
                return View(question);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostAnswer(IFormCollection form)
        {
            int questionId = int.Parse(form["questionId"]);
            string body = form["body"].ToString();
            Answer answer = new Answer()
            {
                Body = body,
                CreateAt = DateTime.Now,
                QuestionId = questionId,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };
            await _context.Answers.AddAsync(answer);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = questionId });
        }

        public IActionResult Ask()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(IFormCollection form)
        {
            Question question = new Question()
            {
                Title = form["title"].ToString(),
                Body = form["body"].ToString(),
                CreateAt = DateTime.Now,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            if (!string.IsNullOrWhiteSpace(form["tags"]))
            {
                var tagNames = form["tags"].ToString()
                    .Split(",")
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Take(4)
                    .ToList();

                foreach (var tagName in tagNames)
                {
                    var existsTag = _context.Tags.FirstOrDefault(x => x.TagName.ToLower() == tagName.ToLower());

                    if (existsTag != null)
                    {
                        question.Tags.Add(existsTag);
                    }
                    else
                    {
                        // Tag mới -Tạo mới và tạo quan hệ
                        var newTag = new Tag()
                        {
                            TagName = tagName,
                        };
                        question.Tags.Add(newTag);
                    }
                }
            }

            await _context.Questions.AddAsync(question);
            _context.SaveChanges();
            return View();
        }
    }
}
