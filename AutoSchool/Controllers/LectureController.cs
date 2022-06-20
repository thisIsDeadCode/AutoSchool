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
                    lectures.Add(lecture.ConvertLectureToLectureResponse());
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
        public async Task<ActionResult<LectureResponse>> Get(long Id, long? themeId = null)
        {
            Lecture? lecture = await _dbContext.Lectures.FirstOrDefaultAsync(x => x.Id == Id);

            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (lecture != null && userDb != null)
            {
                LectureResponse lectureView = lecture.ConvertLectureToLectureResponse();

                //if(themeId != null)
                //{
                //    bool hasNext = false;
                //    bool hasPrevious = false;
                //    var lectures = _dbContext.Lectures.OrderBy(x => x.Id).Where(x => x.ThemeId == themeId).ToList();
                //    var indexCurrentLecture = lectures.IndexOf(lecture);

                //}

                await _historyService.SaveTohistory(userDb, lecture);

                return lectureView;
            }
            else
            {
                return StatusCode(404);
            }
        }

        [HttpGet]
        [Route("Lecture/GetLectureByDirection")]
        public async Task<ActionResult<LectureResponse>> GetLectureByDirection(long themeId, long currentLectureId, string direction)
        {
            User? userDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var lectures = _dbContext.Lectures.OrderBy(x => x.Id).Where(x => x.ThemeId == themeId).ToList();
            var lecture = lectures.FirstOrDefault(x => x.Id == currentLectureId);

            if(userDb != null && lecture != null)
            {
                var indexCurrentLecture = lectures.IndexOf(lecture);

                Lecture resultLecture;
                bool hasNext = false;
                bool hasPrevious = false;
                var countLecture = lectures.Count();

                if (direction == "next")
                {
                    if (countLecture - 1 == indexCurrentLecture)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                        
                    }
                    else
                    {
                        if(countLecture - 1 > indexCurrentLecture + 2 )
                        {
                            hasNext = true; 
                        }
                        resultLecture = lectures[indexCurrentLecture + 1];
                    }
                }
                else if (direction == "previous")
                {
                    if (indexCurrentLecture == 0)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        if (countLecture >= 2)
                        {
                            hasPrevious = true;
                        }
                        resultLecture = lectures[indexCurrentLecture - 1];
                    }
                }
                else
                {
                    return BadRequest();
                }

                LectureResponse resultLectureView = resultLecture.ConvertLectureToLectureResponse();
                resultLectureView.HasPrevious = hasPrevious;
                resultLectureView.NasNext = hasNext;

                await _historyService.SaveTohistory(userDb, resultLecture);

                return resultLectureView;
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

    }
}