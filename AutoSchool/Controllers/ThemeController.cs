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
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<ThemeResponse>>> GetAllThemes(long courseId)
        {
            var themes = _dbContext.Themes.Include(c => c.Test)
                                              .ThenInclude(c => c.ResultTests)
                                              .Include(c => c.Lectures)
                                              .Where(x => x.CourseId == courseId);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);


            if (userDb != null && themes != null && themes.Count() > 0)
            {
                var themesResponse = new List<ThemeResponse>();

                foreach (var theme in themes)
                {
                    themesResponse.Add(theme.ConvertThemeToThemeResponse(userDb));
                }

                return themesResponse;
            }
            else
            {
                return StatusCode(404);
            }
        }

        [HttpGet]
        [Route("Theme/Get")]
        public async Task<ActionResult<ThemeResponse>> Get(long Id)
        {
            Theme? theme = await _dbContext.Themes
                                              .Include(c => c.Test)
                                              .ThenInclude(c => c.ResultTests)
                                              .Include(c => c.Lectures)
                                              .FirstOrDefaultAsync(x => x.Id == Id);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (userDb != null && theme != null && userDb != null)
            {
                ThemeResponse themeView = theme.ConvertThemeToThemeResponse(userDb);

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