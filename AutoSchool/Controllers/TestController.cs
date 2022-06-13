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
    public class TestController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly TestService _testService;
        private readonly HistoryService _historyService;


        public TestController(ILogger<UserController> logger,
            ApplicationDbContext dbContext,
            TestService testService,
            HistoryService historyService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _testService = testService;
            _historyService = historyService;
        }

        [HttpGet]
        [Route("Test/GetTest")]
        public async Task<ActionResult<TestResponse>> GetTest(long themeId)
        {
            //themeId == testId
            User? userDb = await _dbContext.Users
                                            .Include(x => x.Student)
                                            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var test = _dbContext.Tests
                                    .Include(x => x.Questions)
                                    .ThenInclude(x => x.Answers)
                                    .FirstOrDefault(x => x.Id == themeId);

            if (test == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (userDb == null && userDb.Student != null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var testView = await _testService.GenerateTest(test);
            await _historyService.SaveTohistory(userDb, test);

            return testView;
        }

        [HttpPost]
        [Route("Test/SendResult")]
        public async Task<ActionResult<ResultTestResponse>> SendResult(long themeId, [FromBody] List<AnswersToQuestionRequest> answersToQuestionViews)
        {
            //themeId == testId
            User? userDb = await _dbContext.Users
                                            .Include(x => x.Student)
                                            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            var test = _dbContext.Tests
                                .Include(x =>x.Theme)
                                .ThenInclude(x => x.Course)
                                .Include(x => x.Questions)
                                .ThenInclude(x => x.Answers)
                                .FirstOrDefault(x => x.Id == themeId);


            if (test == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (userDb == null && userDb.Student == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }


            var resultTestView = await _testService.CheckTestResults(test, userDb.Student.UserId, answersToQuestionViews);
            return resultTestView;
        }

        [HttpGet]
        [Route("Test/GetTestResult")]
        public async Task<ActionResult<ResultTestResponse>> GetTestResult(long themeId)
        {
            //themeId == testId
            var test = _dbContext.Tests
                                .Include(x => x.ResultTests)
                                .FirstOrDefault(x => x.Id == themeId);

            User? userDb = await _dbContext.Users
                                            .Include(x => x.Student)
                                            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (test == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if (userDb == null && userDb.Student != null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var resultTest = test.ResultTests
                                    .OrderByDescending(x => x.Date)
                                    .Where(x => x.Result == 1 && x.StudentUserId == userDb.Student.UserId)
                                    .FirstOrDefault();
            if (resultTest == null)
            {
                resultTest = test.ResultTests
                                    .OrderByDescending(x => x.Date)
                                    .FirstOrDefault();
            }

            return resultTest.ConvertResultTestToResultTestViewView();
        }
    }
}