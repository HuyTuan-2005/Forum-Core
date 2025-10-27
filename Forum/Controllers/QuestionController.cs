using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            return View(_context.Questions.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue)
            {
                var question = await _context.Questions
                    .Include(q => q.User)       
                    .Include(q => q.Tags)    
                    .FirstOrDefaultAsync(q => q.QuestionId == id.Value);
                return View(_context.Questions.FirstOrDefault(t => t.QuestionId == id.Value));
            }
            return RedirectToAction("Index");
        }

        public IActionResult Ask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(IFormCollection form)
        {
            Question question = new Question()
            {
                Title = form["title"].ToString(),
                Body = form["body"].ToString(),
                CreateAt = DateTime.Now,
                UserId = 1
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
