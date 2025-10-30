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
        public async Task<IActionResult> Index(string search, int? page = 1)
        {
            List<Question> lstQuestions = new List<Question>();
            int pageSize = 10;
            
            if (!string.IsNullOrEmpty(search))
            {
                lstQuestions = await _context.Questions
                    .Include(q => q.User).ThenInclude(u => u.Profile)
                    .Include(q => q.Tags)
                    .Include(q => q.Answer)
                    .Where(q => q.Title.Contains(search) || q.Body.Contains(search))
                    .OrderByDescending(q => q.CreateAt)
                    .ToListAsync();
            }
            else
            {
                lstQuestions = _context.Questions
                    .Include(q => q.User).ThenInclude(u => u.Profile)
                    .Include(q => q.Tags)
                    .Include(q => q.Answer)
                    .OrderByDescending(q => q.CreateAt)
                    .ToList();
            }

            ViewData["CurrentPage"] = page.Value;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)lstQuestions.Count / pageSize);
                
            lstQuestions = lstQuestions.Skip((page.Value - 1) * pageSize).Take(pageSize).ToList();
            return View(lstQuestions);
        }
        
        public async Task<IActionResult> Search(string keyword)
        {
            var lstQuestions = await _context.Questions
                .Include(q => q.User).ThenInclude(u => u.Profile)
                .Include(q => q.Tags)
                .Include(q => q.Answer)
                .Where(q => q.Title.Contains(keyword) || q.Body.Contains(keyword))
                .OrderByDescending(q => q.CreateAt)
                .ToListAsync();
            return View("Index", lstQuestions);
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
            string title = form["title"].ToString();
            string body = form["body"].ToString();
            if(title.Length > 255)
            {
                ModelState.AddModelError(string.Empty, "Tiêu đề không được vượt quá 255 ký tự.");
                return View();
            }
            
            Question question = new Question()
            {
                Title = title,
                Body = body,
                CreateAt = DateTime.Now,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            if (!string.IsNullOrWhiteSpace(form["tags"]))
            {
                var tagNames = form["tags"].ToString()
                    .Split(",")
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x) && x.Length <= 20)
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
            return RedirectToAction("Details", new { id = question.QuestionId });
        }
    }
}
