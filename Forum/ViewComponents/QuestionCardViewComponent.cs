using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.ViewComponents;

public class QuestionCardViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    
    public QuestionCardViewComponent(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(Question question, int? questionId = null)
    {
        if (questionId.HasValue)
        {
            var q = await _context.Questions
                .Include(q => q.User)
                .Include(a => a.Answer)
                .Include(q => q.Tags)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId.Value);
            
            return View(q);
        }
        
        return View(question);
    }
    
}