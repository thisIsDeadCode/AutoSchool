using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using static AutoSchool.Models.Views.LinksLeftMenuResponse;

namespace AutoSchool.Extensions
{
    public static class Converter
    {
        public static CourseResponse ConvertCourseToCourseView(this Course course)
        {
            var courseView = new CourseResponse()
            {
                Id = course.Id,
                Name = course.Name,
                Progress = 0,
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

        public static LinksLeftMenuResponse ConvertVisitHistoriesToLinksLeftMenu(this IEnumerable<VisitHistory> visitHistories)
        {
            var linksLeftMenuView = new LinksLeftMenuResponse();

            var courses = visitHistories.Where(x => x.CourseId != null).ToList();
            var themes = visitHistories.Where(x => x.ThemeId != null).ToList();
            var lectures = visitHistories.Where(x => x.LectureId != null).ToList();
            var tests = visitHistories.Where(x => x.TestId != null).ToList();


            linksLeftMenuView.HrefCourses = courses.Select(x => new Link 
            { 
                Title = x.Course.Name,
                Href = $"Course/Get?Id={x.CourseId}"
            }).ToList();

            linksLeftMenuView.HrefThemes = themes.Select(x => new Link
            {
                Title = x.Theme.Name,
                Href = $"Theme/Get?Id={x.ThemeId}"
            }).ToList();

            linksLeftMenuView.HrefLectures = lectures.Select(x => new Link
            {
                Title = x.Lecture.Name,
                Href = $"Lecture/Get?Id={x.LectureId}"
            }).ToList();

            linksLeftMenuView.HrefTests = tests.Select(x => new Link
            {
                Title = x.Test.Name,
                Href = $"Test/Get?Id={x.TestId}"
            }).ToList();

            return linksLeftMenuView;
        }

        public static ThemeResponse ConvertThemeToThemeView(this Theme theme)
        {
            var themeView = new ThemeResponse()
            {
                Id = theme.Id,
                Name = theme.Name,
                Description = theme.Description,
                AmountLecture = theme.Lectures.Count()
            };

            var lastResult = theme.Test?.ResultTests?.OrderByDescending(x => x.Date).FirstOrDefault();

            var lastRightResult = theme.Test?.ResultTests?
                                        .Where(x => x.Test.ResultTests.Any(z => z.Result == 1))
                                        .OrderByDescending(x => x.Date).FirstOrDefault();

            if(lastResult == null && lastRightResult == null)
            {
                themeView.Status = "Тест не начат";
            }
            else
            {
                if(lastRightResult == null)
                {
                    themeView.Status = "Тест не пройден";
                    themeView.TestDate = null;
                }
                else
                {
                    themeView.Status = "Тест пройден";
                    themeView.TestDate = lastRightResult.Date;
                }
            }
           
            return themeView;
        }

        public static LectureResponse ConvertLectureToLectureView(this Lecture lecture)
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

        public static QuestionResponse ConvertQuestionToQuestionView(this Question question)
        {
            var questionView = new QuestionResponse()
            {
                Id = question.Id,
                QuestionImageId = question.QuestionImageId,
                QuestionText = question.QuestionText,
            };

            return questionView;
        }

        public static ResultTestResponse ConvertResultTestToResultTestViewView(this ResultTest resultTest)
        {
            var resultTestView = new ResultTestResponse()
            {
                Id = resultTest.Id,
                AmountRightQuestions = resultTest.AmountRightQuestions,
                AmountWrongQuestions = resultTest.AmountWrongQuestions,
                Date = resultTest.Date,
                Result = resultTest.Result,
                Status = resultTest.Status,
            };

            return resultTestView;
        }
    }
}
