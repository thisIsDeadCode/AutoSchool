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
    public class LectureController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly HistoryService _historyService;

        public LectureController(ILogger<UserController> logger,
            ApplicationDbContext dbContext, 
            HistoryService historyService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _historyService = historyService;
        }

        [HttpGet]
        [Route("Lecture/GetAllLectures")]
        public async Task<ActionResult<IEnumerable<LectureResponse>>> GetAllLecture(long themeId)
        {
            Theme? theme =  await  _dbContext.Themes
                                    .Include(c => c.Lectures)
                                    .FirstOrDefaultAsync(x=>x.Id == themeId); 
            if(theme != null)
            {
                var lectures = new List<LectureResponse>();

                foreach (var lecture in theme.Lectures)
                {
                    lectures.Add(lecture.ConvertLectureToLectureView());
                }

                return lectures;
            }
            else
            {
                return StatusCode(404);
            }
        }

        [HttpGet]
        [Route("Lecture/Get")]
        public async Task<ActionResult<LectureResponse>> Get(long Id)
        {
            Lecture? lecture = await _dbContext.Lectures.FirstOrDefaultAsync(x => x.Id == Id);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (lecture != null && userDb != null)
            {
                LectureResponse lectureView = lecture.ConvertLectureToLectureView();

                await _historyService.SaveTohistory(userDb, lecture);

                return lectureView;
            }
            else
            {
                return StatusCode(404);
            }
        }

    }
}