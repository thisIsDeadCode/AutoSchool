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
        public DbSet<File> Files { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasKey("Id");
            modelBuilder.Entity<File>().HasKey("Id");
            modelBuilder.Entity<Student>().HasKey("UserId");
            modelBuilder.Entity<StudentsCourses>().HasKey("StudentId", "CourseId");
            modelBuilder.Entity<Teacher>().HasKey("UserId");
            modelBuilder.Entity<User>().HasKey("Id");


            modelBuilder.Entity<User>().HasOne(u => u.Student).WithOne(u => u.User);
            modelBuilder.Entity<User>().HasOne(t => t.Teacher).WithOne(u => u.User);

            modelBuilder.Entity<Course>().HasOne(t => t.Teacher).WithMany(c => c.Courses);

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

            foreach (var student in students)
            {
                studentsCourses.Add(new StudentsCourses()
                {
                    CourseId = rnd.Next(1, 10),
                    StudentId = student.UserId,
                });
            }

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Teacher>().HasData(teachers);
            modelBuilder.Entity<Student>().HasData(students);
            modelBuilder.Entity<Course>().HasData(courses);
            modelBuilder.Entity<StudentsCourses>().HasData(studentsCourses);


            base.OnModelCreating(modelBuilder);
        }
    }
}
