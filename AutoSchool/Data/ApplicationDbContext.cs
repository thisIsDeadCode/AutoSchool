using AutoSchool.Models.Tables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using File = AutoSchool.Models.Tables.File;

namespace AutoSchool.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentsCourses> StudentsCourses { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<ResultTest> ResultTests { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<VisitHistory> VisitHistories { get; set; }
        public DbSet<QuestionAnswers> QuestionAnswers { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>().HasKey("Id");
            modelBuilder.Entity<Course>().HasKey("Id");
            modelBuilder.Entity<File>().HasKey("Id");
            modelBuilder.Entity<Lecture>().HasKey("Id");
            modelBuilder.Entity<Question>().HasKey("Id");
            modelBuilder.Entity<QuestionAnswers>().HasKey("Id");
            modelBuilder.Entity<Student>().HasKey("UserId");
            modelBuilder.Entity<StudentsCourses>().HasKey("StudentId", "CourseId");
            modelBuilder.Entity<Teacher>().HasKey("UserId");
            modelBuilder.Entity<Test>().HasKey("Id");
            modelBuilder.Entity<Theme>().HasKey("Id");
            modelBuilder.Entity<User>().HasKey("Id");

            modelBuilder.Entity<User>().HasOne(u => u.Student).WithOne(u => u.User);
            modelBuilder.Entity<User>().HasOne(t => t.Teacher).WithOne(u => u.User);
            modelBuilder.Entity<User>().HasMany(t => t.Files).WithOne(u => u.User);

            modelBuilder.Entity<Course>().HasOne(t => t.Teacher).WithMany(c => c.Courses);
            modelBuilder.Entity<Course>().HasMany(t => t.Themes).WithOne(c => c.Course).HasForeignKey(x => x.CourseId);

            modelBuilder.Entity<Theme>().HasOne(t => t.Test).WithOne(c => c.Theme).HasForeignKey<Test>(x=>x.Id);
            modelBuilder.Entity<Theme>().HasMany(t => t.Lectures).WithOne(c => c.Theme);

            modelBuilder.Entity<Test>().HasMany(t => t.Questions).WithOne(c => c.Test);
            modelBuilder.Entity<Question>().HasMany(t => t.Answers).WithOne(c => c.Question);

            modelBuilder.Entity<VisitHistory>().HasOne(t => t.Course).WithMany(c => c.Visits).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<VisitHistory>().HasOne(t => t.Theme).WithMany(c => c.Visits).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<VisitHistory>().HasOne(t => t.Lecture).WithMany(c => c.Visits).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<VisitHistory>().HasOne(t => t.Test).WithMany(c => c.Visits).OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<QuestionAnswers>().HasOne(t => t.ResultTest).WithMany(c => c.QuestionAnswers).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<QuestionAnswers>().HasOne(t => t.Question).WithMany(c => c.QuestionAnswers).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<QuestionAnswers>().HasOne(t => t.Answer).WithMany(c => c.QuestionAnswers).OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<ResultTest>()
                .HasOne(t => t.Student)
                .WithMany(c => c.ResultTests)
                .HasForeignKey(x => x.StudentUserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ResultTest>()
                .HasOne(t => t.Test)
                .WithMany(c => c.ResultTests)
                .HasForeignKey(x => x.TestId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentsCourses>()
                .HasOne(s => s.Course)
                .WithMany(c => c.StudentsCoursies)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<StudentsCourses>()
                .HasOne(s => s.Student)
                .WithMany(c => c.StudentsCoursies)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            var users = new List<User>();
            var teachers = new List<Teacher>();
            var students = new List<Student>();
            var courses = new List<Course>();
            var studentsCourses = new List<StudentsCourses>();
            var themes = new List<Theme>();
            var lectures = new List<Lecture>();


            for (int i = 1; i < 101; i++)
            {
                var user = new User
                {
                    Id = i,
                    UserName = $"Test User{i}",
                    FullName = $"Full Name{i}",
                    Email = $"TestUser{i}@mail.ru",
                    EmailConfirmed = true,
                    Password = $"TestUser{i}@mail.ru",
                    PhoneNumber = null,
                };

                if (i <= 10)
                {
                    var teacher = new Teacher
                    {
                        UserId = i,
                    };

                    var course = new Course
                    {
                        Id = i,
                        Name = $"Course{i}",
                        Description = $"Description{i}",
                        TeacherId = teacher.UserId
                    };
                                      

                    courses.Add(course);
                    teachers.Add(teacher);
                }
                else
                {
                    var student = new Student
                    {
                        UserId = i,
                    };

                    students.Add(student);
                }

                users.Add(user);    
            }

            Random rnd = new Random();

            for (int i = 1; i <= 100; i++)
            {
                var x = rnd.Next(1, 10);
                themes.Add(new Theme()
                {
                    Id = i,
                    Name = $"Course{x} - Theme{i}",
                    Description = $"Course{x} - Theme{i} description",
                    CourseId = rnd.Next(1, 10)
                });
            }

            for (int i = 1; i <= 1000; i++)
            {
                lectures.Add(new Lecture()
                {
                    Id = i,
                    Name = $"Lecture{i}",
                    Description = $"Description lecture{i}",
                    TextHTML = "HTML",
                    ThemeId = rnd.Next(1, 100),
                });
            }

            foreach (var student in students)
            {
                studentsCourses.Add(new StudentsCourses()
                {
                    CourseId = rnd.Next(1, 10),
                    StudentId = student.UserId,
                    Progress = rnd.Next(0, 100),
                    Status = "status"
                });
            }

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Teacher>().HasData(teachers);
            modelBuilder.Entity<Student>().HasData(students);
            modelBuilder.Entity<Course>().HasData(courses);
            modelBuilder.Entity<StudentsCourses>().HasData(studentsCourses);
            modelBuilder.Entity<Theme>().HasData(themes);
            modelBuilder.Entity<Lecture>().HasData(lectures);


            base.OnModelCreating(modelBuilder);
        }
    }
}
