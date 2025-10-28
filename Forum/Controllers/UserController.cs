using Forum.Data;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View("Login");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection)
        {
            var user = await _userManager.FindByNameAsync(collection["username"]);
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, collection["password"], false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(collection);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            
            return RedirectToAction("Index", "Question");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var profile = new Profile()
            {
                Birthday = Convert.ToDateTime("1/1/1790"),
                DisplayName = "User",
                Gender = "Nam"
            };

            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                Profile = profile
            };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Question");
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Question");
        }

    }
}
