﻿using AutoSchool.Data;
using AutoSchool.Models.Tables;
using AutoSchool.Models.Views;

namespace AutoSchool.Extensions
{
    public static class CourseViewExtensions
    {
        public static void LoadProgressToCourses(this List<CourseView> courses, ApplicationDbContext dbContext,  long UserId)
        {
            List<StudentsCourses> studentsCourses = dbContext.StudentsCourses.Where(x=>x.StudentId == UserId).ToList();


            foreach (CourseView course in courses)
            {
                var ctc = studentsCourses.FirstOrDefault(ctc => ctc.CourseId == course.Id);
                course.Progress = ctc == null ? 0 : ctc.Progress;
            }
        }

        public static void LoadProgressToCourse(this CourseView course, ApplicationDbContext dbContext, long UserId)
        {
            StudentsCourses? studentCourse = dbContext.StudentsCourses.FirstOrDefault(x => x.StudentId == UserId && x.CourseId == course.Id);
            course.Progress = studentCourse == null ? 0 : studentCourse.Progress;
        }
    }
}
