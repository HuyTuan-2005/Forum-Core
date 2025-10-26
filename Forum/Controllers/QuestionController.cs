using Forum.Data;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Ask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ask(FormCollection collection)
        {

            return View();
        }
    }
}
