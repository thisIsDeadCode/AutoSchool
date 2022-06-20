using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using static AutoSchool.Models.Views.LinksLeftMenuResponse;

namespace AutoSchool.Extensions
{
    public static class Converter
    {
        public static CourseResponse ConvertCourseToCourseResponse(this Course course)
        {
            long amountLecture = 0;
            foreach (var theme in course.Themes)
            {
                amountLecture += theme.Lectures.Count();
            }

            var courseView = new CourseResponse()
            {
                Id = course.Id,
                Name = course.Name,
                Status = "Курс не начат",
                Progress = 0,
                AmountLecture = amountLecture,
                AmountThemes = course.Themes.Count(),
                Teacher = new TeacherResponse()
                {
                    Id = course.Teacher.UserId,
                    UserName = course.Teacher.User.UserName,
                    Email = course.Teacher.User.Email,
                    FullName = course.Teacher.User.FullName,
                    Location = course.Teacher.User.Location,
                    PhoneNumber = course.Teacher.User.PhoneNumber,
                    PhotoId = null,
                    FacebookURL = course.Teacher.User.FacebookURL,
                    UserNameInstagram = course.Teacher.User.UserNameInstagram,
                    UserNameTelegram = course.Teacher.User.UserNameTelegram,
                    UserNameTwitter = course.Teacher.User.UserNameTwitter,
                },
                Description = course.Description,
                Students = course.StudentsCoursies.Select(s => new StudentResponse()
                {
                    Id = s.Student.UserId,
                    UserName = s.Student.User.UserName,
                    Email = s.Student.User.Email,
                    FullName = s.Student.User.FullName,
                    Location = s.Student.User.Location,
                    PhoneNumber = s.Student.User.PhoneNumber,
                    PhotoId = null,
                    FacebookURL = s.Student.User.FacebookURL,
                    UserNameInstagram = s.Student.User.UserNameInstagram,
                    UserNameTelegram = s.Student.User.UserNameTelegram,
                    UserNameTwitter = s.Student.User.UserNameTwitter,
                }).ToList()
            };

            return courseView;
        }

        public static LinksLeftMenuResponse ConvertVisitHistoriesToLinksLeftMenuResponse(this IEnumerable<VisitHistory> visitHistories)
        {
            var linksLeftMenuView = new LinksLeftMenuResponse();

            var courses = visitHistories.Where(x => x.CourseId != null).ToList();
            var themes = visitHistories.Where(x => x.ThemeId != null).ToList();
            var lectures = visitHistories.Where(x => x.LectureId != null).ToList();
            var tests = visitHistories.Where(x => x.TestId != null).ToList();


            linksLeftMenuView.HrefCourses = courses.Select(x => new LinkResponse
            {
                Title = x.Course.Name,
                Href = $"Course/Get?Id={x.CourseId}"
            }).ToList();

            linksLeftMenuView.HrefThemes = themes.Select(x => new LinkResponse
            {
                Title = x.Theme.Name,
                Href = $"Theme/Get?Id={x.ThemeId}"
            }).ToList();

            linksLeftMenuView.HrefLectures = lectures.Select(x => new LinkResponse
            {
                Title = x.Lecture.Name,
                Href = $"Lecture/Get?Id={x.LectureId}"
            }).ToList();

            linksLeftMenuView.HrefTests = tests.Select(x => new LinkResponse
            {
                Title = x.Test.Name,
                Href = $"Test/Get?Id={x.TestId}"
            }).ToList();

            return linksLeftMenuView;
        }

        public static ThemeResponse ConvertThemeToThemeResponse(this Theme theme, User user)
        {
            var themeView = new ThemeResponse()
            {
                Id = theme.Id,
                Name = theme.Name,
                Description = theme.Description,
                AmountLecture = theme.Lectures.Count()
            };

            var lastResult = theme.Test?.ResultTests?.OrderByDescending(x => x.Date).FirstOrDefault(x => x.StudentUserId == user.Id);

            var lastRightResult = theme.Test?.ResultTests?
                                        .OrderByDescending(x => x.Date)
                                        .FirstOrDefault(x => x.StudentUserId == user.Id && x.Result == 1);

            if (lastResult == null && lastRightResult == null)
            {
                themeView.Status = "Тест не начат";
                themeView.IsActiveButtonLastResultTest = false;
            }
            else
            {
                if (lastRightResult == null)
                {
                    themeView.Status = "Тест не пройден";
                    themeView.TestDate = null;
                }
                else
                {
                    themeView.Status = "Тест пройден";
                    themeView.TestDate = lastRightResult.Date;
                }
                themeView.IsActiveButtonLastResultTest = true;
            }

            return themeView;
        }

        public static LectureResponse ConvertLectureToLectureResponse(this Lecture lecture)
        {
            var lectureView = new LectureResponse()
            {
                Id = lecture.Id,
                Name = lecture.Name,
                TextHTML = lecture.TextHTML,
                Description = lecture.Description,
            };

            return lectureView;
        }

        public static QuestionResponse ConvertQuestionToQuestionResponse(this Question question)
        {
            var questionView = new QuestionResponse()
            {
                Id = question.Id,
                QuestionImageId = question.QuestionImageId,
                QuestionText = question.QuestionText,
                Answers = new List<AnswerResponce>()
            };

            foreach (var answer in question.Answers)
            {
                questionView.Answers.Add(new AnswerResponce()
                {
                    AnswerId = answer.Id,
                    TextAnswer = answer.TextAnswer,
                });
            }

            return questionView;
        }

        public static ResultTestResponse ConvertResultTestToResultTestResponse(this ResultTest resultTest)
        {
            var resultTestView = new ResultTestResponse()
            {
                Id = resultTest.Id,
                AmountRightQuestions = resultTest.AmountRightQuestions,
                AmountWrongQuestions = resultTest.AmountWrongQuestions,
                Date = resultTest.Date,
                Result = resultTest.Result * 100,
                Status = resultTest.Status,
            };

            return resultTestView;
        }

        public static List<GradesResponse> ConvertCourseToGradesResponse(this Course course)
        {
            var courseGrades = new List<GradesResponse>();

            foreach (var studentsCoursies in course.StudentsCoursies)
            {
                var userGrades = new GradesResponse()
                {
                    CourseId = course.Id,
                    NameCourse = course.Name,
                    UserId = studentsCoursies.Student.User.Id,
                    FullName = studentsCoursies.Student.User.FullName,
                    Email = studentsCoursies.Student.User.Email,
                };
                userGrades.Grades = new List<GradeResponse>();
                var resultTests = studentsCoursies.Student.ResultTests;

                if (resultTests != null)
                {
                    foreach (var testResult in studentsCoursies.Student.ResultTests)
                    {
                        userGrades.Grades.Add(new GradeResponse()
                        {
                            ThemeId = testResult.Test.Theme.Id,
                            NameTheme = testResult.Test.Theme.Name,
                            Progress = testResult.Result * 100,
                            Status = testResult.Status,
                            Date = testResult.Date
                        });
                    }
                    courseGrades.Add(userGrades);
                }
            }

            return courseGrades;
        }
    }
}
