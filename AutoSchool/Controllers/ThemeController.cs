using AutoSchool.Data;
using AutoSchool.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoSchool.Extensions;
using AutoSchool.Models.Tables;
using Microsoft.AspNetCore.Authorization;
using AutoSchool.Services;

namespace AutoSchool.Controllers
{
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly HistoryService _historyService;

        public ThemeController(ILogger<UserController> logger,
            ApplicationDbContext dbContext, 
            HistoryService historyService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _historyService = historyService;
        }

        [HttpGet]
        [Route("Theme/GetAllThemes")]
        public async Task<ActionResult<IEnumerable<ThemeView>>> GetAllThemes(long courseId)
        {
            Course? course =  await  _dbContext.Courses
                                    .Include(c => c.Themes)
                                    .ThenInclude(x => x.Lectures)
                                    .FirstOrDefaultAsync(x=>x.Id == courseId); 
            if(course != null)
            {
                var themes = new List<ThemeView>();

                foreach (var theme in course.Themes)
                {                    
                    themes.Add(theme.ConvertThemeToThemeView());
                }

                return themes;
            }
            else
            {
                return StatusCode(404);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Theme/Get")]
        public async Task<ActionResult<ThemeView>> Get(long Id)
        {
            Theme? theme = await _dbContext.Themes
                                              .Include(c => c.Test)   
                                              .ThenInclude(c => c.ResultTests)
                                              .Include(c => c.Lectures)
                                              .FirstOrDefaultAsync(x => x.Id == Id);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (theme != null && userDb != null)
            {
                ThemeView themeView = theme.ConvertThemeToThemeView();

                await _historyService.SaveTohistory(userDb, theme);

                return themeView;
            }
            else
            {
                return StatusCode(404);
            }
        }

    }
}