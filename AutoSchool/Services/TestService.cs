using AutoSchool.Data;
using AutoSchool.Extensions;
using AutoSchool.Models.Interfaces;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace AutoSchool.Services
{
    public class TestService
    {
        private readonly ApplicationDbContext _dbContext;

        public TestService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TestResponse> GenerateTest(Test test)
        {
            List<QuestionResponse> questionsView = new List<QuestionResponse>();

            if (test.AmountQuestions >= test.Questions.Count())
            {
                foreach (var question in test.Questions)
                {
                    questionsView.Add(question.ConvertQuestionToQuestionView());
                }
            }
            else
            {
                Random random = new Random();
                var tmpQuestions = test.Questions.ToList();
                var max = tmpQuestions.Count();

                for (int i = 0; i < test.AmountQuestions; i++)
                {
                    var index = random.Next(0, max);
                    questionsView.Add(tmpQuestions[index].ConvertQuestionToQuestionView());
                }
            }

            var testView = new TestResponse()
            {
                Id = test.Id,
                Name = test.Name,
                Description = test.Description,
                Questions = questionsView
            };

            return testView;
        }

        public async Task<ResultTestResponse> CheckTestResults(Test test, long userId, List<AnswersToQuestionRequest> answersToQuestionView)
        {
            var resultTestView = new ResultTestResponse();
            resultTestView.Errors = new List<string>();

            var questions = test.Questions.ToList();

            if (questions == null || questions.Count() == 0)
            {
                resultTestView.Errors.Add("Ошибка, у этого теста нет вопросов");
                return resultTestView;
            }

            List<QuestionAnswers> questionAnswers = new List<QuestionAnswers>();
            var resultTest = new ResultTest();

            var amountRightQuestions = 0;
            var amountWrongQuestions = 0;

            foreach (var answersToQuestion in answersToQuestionView)
            {
                var question = questions.FirstOrDefault(x => x.Id == answersToQuestion.QuestionId);

                if (question == null)
                {
                    resultTestView.Errors.Add("Ошибка, не верный вопрос");
                    amountWrongQuestions++;
                }
                else
                {
                    if (question.Id == answersToQuestion.QuestionId)
                    {
                        var rightAnswers = question.Answers.Where(x => x.IsRight).ToList();
                        var amountRightAnswers = 0;

                        if (answersToQuestion.Answers.Count() == rightAnswers.Count())
                        {
                            foreach (var answer in answersToQuestion.Answers)
                            {
                                var rightAnswer = rightAnswers.FirstOrDefault(x => x.Id == answer.AnswerId);
                                if (rightAnswer != null)
                                {
                                    amountRightAnswers++;

                                    questionAnswers.Add(new QuestionAnswers()
                                    {
                                        ResultTest = resultTest,
                                        QuestionId = question.Id,
                                        AnswerId = answer.AnswerId,
                                    });
                                }
                            }
                        }

                        if (amountRightAnswers == rightAnswers.Count())
                        {
                            amountRightQuestions++;
                        }
                        else
                        {
                            amountWrongQuestions++;
                        }
                    }
                }
            }

            if (test.AmountQuestions != amountWrongQuestions + amountRightQuestions)
            {
                resultTestView.Errors.Add("Ошибка,количество вопросов и ответов не совпадает");
            }
            else
            {

                double result = amountRightQuestions / test.AmountQuestions;

                resultTest.Status = result == 1 ? "Тест пройден" : "Тест не пройден";
                resultTest.AmountRightQuestions = amountRightQuestions;
                resultTest.AmountWrongQuestions = amountWrongQuestions;
                resultTest.Result = result;
                resultTest.Date = DateTime.Now;
                resultTest.StudentUserId = userId;
                resultTest.TestId = test.Id;
                resultTest.QuestionAnswers = questionAnswers;

                _dbContext.ResultTests.Add(resultTest);
                _dbContext.QuestionAnswers.AddRange(questionAnswers);
                await _dbContext.SaveChangesAsync();


                var course = test.Theme.Course;
                var studentCourse = await _dbContext.StudentsCourses.FirstOrDefaultAsync(x => x.CourseId == course.Id && x.StudentId == userId);

                var themes = _dbContext.Themes.Include(x => x.Test)
                                            .ThenInclude(x => x.ResultTests)
                                            .Where(x => x.CourseId == course.Id).ToList();

                studentCourse.Status = "Курс начат";

                if (result == 1)
                {
                    var countTheme = themes.Count();
                    var countFinishedTheme = themes.Where(x => x.Test != null && x.Test.ResultTests.Any(z => z.Result == 1)).Count();

                    studentCourse.Progress = (countFinishedTheme / (double)countTheme) * 100;

                    if (studentCourse.Progress == 1)
                    {
                        studentCourse.Status = "Курс пройден";
                    }
                }

                _dbContext.StudentsCourses.Update(studentCourse);
                await _dbContext.SaveChangesAsync();

                resultTestView.Id = resultTest.Id;
                resultTestView.Status = resultTest.Status;
                resultTestView.AmountRightQuestions = resultTest.AmountRightQuestions;
                resultTestView.AmountWrongQuestions = resultTest.AmountWrongQuestions;
                resultTestView.Result = resultTest.Result;
                resultTestView.Date = resultTest.Date;
            }

            return resultTestView;
        }
    }
}
