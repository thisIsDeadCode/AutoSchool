using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;
using static AutoSchool.Models.Views.LinksLeftMenuView;

namespace AutoSchool.Extensions
{
    public static class Converter
    {
        public static CourseView ConvertCourseToCourseView(this Course course)
        {
            var courseView = new CourseView()
            {
                Id = course.Id,
                Name = course.Name,
                Progress = 0,
                Teacher = new TeacherView()
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
                Students = course.StudentsCoursies.Select(s => new StudentView()
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

        public static LinksLeftMenuView ConvertVisitHistoriesToLinksLeftMenu(this IEnumerable<VisitHistory> visitHistories)
        {
            var linksLeftMenuView = new LinksLeftMenuView();

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

        public static ThemeView ConvertThemeToThemeView(this Theme theme)
        {
            var themeView = new ThemeView()
            {
                Id = theme.Id,
                Name = theme.Name,
                Description = theme.Description,
                AmountLecture = theme.Lectures.Count()
            };

            var lastResult = theme.Test?.ResultTests?.OrderByDescending(x => x.Date).FirstOrDefault();

            var lastRightResult = theme.Test?.ResultTests?
                                        .Where(x => x.Test.ResultTests.Any(z => z.TestResult == 1))
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

        public static LectureView ConvertLectureToLectureView(this Lecture lecture)
        {
            var lectureView = new LectureView()
            {
                Id = lecture.Id,
                Name = lecture.Name,
                TextHTML = lecture.TextHTML,
                Description = lecture.Description,
            };

            return lectureView;
        }

        public static QuestionView ConvertQuestionToQuestionView(this Question question)
        {
            var questionView = new QuestionView()
            {
                Id = question.Id,
                QuestionImageId = question.QuestionImageId,
                QuestionText = question.QuestionText,
            };

            return questionView;
        }
    }
}
