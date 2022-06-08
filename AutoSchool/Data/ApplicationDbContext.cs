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
            var tests = new List<Test>();
            var questions = new List<Question>();
            var answers = new List<Answer>();


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

            courses.Add(new Course
            {
                Id = 11,
                Name = $"Первозка опасных грузов",
                Description = $"Первозка опасных грузов автомобильным транспортом в области международных авто-перевозок",
                TeacherId = 1
            });

            themes.Add(new Theme()
            {
                Id = 5000,
                Name = $"Классификация опасных грузов",
                CourseId = 11
            });
            themes.Add(new Theme()
            {
                Id = 5001,
                Name = $"Грузы повышенной опасности и разрешительная система при их перевозках",
                CourseId = 11
            }); 
            themes.Add(new Theme()
            {
                Id = 5002,
                Name = $"Маркировка изделий и упаковок с опасными грузами",
                CourseId = 11
            });
            lectures.Add(new Lecture()
            {
                Id = 10000,
                Name = $"2 Классификация",
                TextHTML = "В соответствии с ДОПОГ предусматриваются следующие классы опасных грузов:\n" +
                "1 Класс 1 Взрывчатые вещества и изделия\n" +
                "2 Класс 2 Газы\n" +
                "3 Класс 3 Легковоспламеняющиеся жидкости\n" +
                "4 Класс 4.1 Легковоспламеняющиеся твердые вещества, самореактивные вещества и твердые десенсибилизированные взрывчатые вещества\n" +
                "5 Класс 4.2 Вещества, способные к самовозгоранию\n" +
                "6 Класс 4.3 Вещества,выделяющие легковоспламеняющиеся газы при соприкосновении с водой\n" +
                "7 Класс 5.1 Окисляющие вещества\n" +
                "8 Класс 5.2 Органические пероксиды\n" +
                "9 Класс 6.1 Токсичные вещества\n" +
                "10Класс 6.2 Инфекционные вещества\n" +
                "11Класс 7 Радиоактивные материалы\n" +
                "12Класс 8 Коррозионные вещества\n" +
                "13Класс 9 Прочие опасные вещества и изделия\n",
                ThemeId = 5000,
            });
            lectures.Add(new Lecture()
            {
                Id = 10001,
                Name = $"6 Грузы повышенной опасности",
                TextHTML = "Грузы повышенной опасности." +
                "Грузы повышенной опасности являются грузы, которые могут быть использованы не поназначению," +
                "а в террористических целях, и, следовательно, привести к серьезным последствиям, таким как многочисленные людские потери," +
                "массовые разрушения или, массовые социальноэкономические потрясения",
                ThemeId = 5001,
            });
            lectures.Add(new Lecture()
            {
                Id = 10002,
                Name = $"10 Маркировка автомобилей\n",
                TextHTML = "Маркировка транспортных единиц.\n" +
                "Маркировка табличками оранжевого цвета" +
                "Транспортные единицы, перевозящие опасные грузы, должны иметь две расположенные в " +
                "вертикальной плоскости прямоугольные таблички оранжевого цвета.Одна из этих табличек должна крепиться спереди," +
                "а другая – сзади транспортной единицы, причем обе – перпендикулярно продольной оси транспортной единицы.Они должны быть хорошо видны.",
                ThemeId = 5002,
            });

            tests.Add(new Test()
            {
                Id = 5000,
                Name = "Классификация опасных грузов",
                Description = null,
                AmountQuestions = 3,
            });

            questions.Add(new Question()
            {
                Id = 1,
                QuestionImageId = null,
                QuestionText = "К какому классу опасных грузов в соответствии с ДОПОГ относятся взрывчатые вещества и изделия, которые содержат такие вещества?",
                TestId = 5000,
            });
            questions.Add(new Question()
            {
                Id = 2,
                QuestionImageId = null,
                QuestionText = "К какому классу опасных грузов в соответствии с ДОПОГ относятся пиротехнические вещества?",
                TestId = 5000,
            });
            questions.Add(new Question()
            {
                Id = 3,
                QuestionImageId = null,
                QuestionText = "К какому классу опасных грузов в соответствии с ДОПОГ относятся пиротехнические изделия?",
                TestId = 5000,
            });

            answers.Add(new Answer()
            {
                Id = 1,
                IsRight = false,
                QuestionId = 1,
                TextAnswer = "1. К классу 4.1.",
            });
            answers.Add(new Answer()
            {
                Id = 2,
                IsRight = true,
                QuestionId = 1,
                TextAnswer = "2. К классу 1.",
            });
            answers.Add(new Answer()
            {
                Id = 3,
                IsRight = false,
                QuestionId = 1,
                TextAnswer = "3. К классу 3.",
            });
            answers.Add(new Answer()
            {
                Id = 4,
                IsRight = false,
                QuestionId = 1,
                TextAnswer = "4. К классу 2.",
            });

            answers.Add(new Answer()
            {
                Id = 5,
                IsRight = false,
                QuestionId = 2,
                TextAnswer = "1. К классу 4.1",
            });
            answers.Add(new Answer()
            {
                Id = 6,
                IsRight = true,
                QuestionId = 2,
                TextAnswer = "2. К классу 1.",
            });
            answers.Add(new Answer()
            {
                Id = 7,
                IsRight = false,
                QuestionId = 2,
                TextAnswer = "3. К классу 5.2.",
            });
            answers.Add(new Answer()
            {
                Id = 8,
                IsRight = false,
                QuestionId = 2,
                TextAnswer = "4. К классу 2.",
            });

            answers.Add(new Answer()
            {
                Id = 9,
                IsRight = false,
                QuestionId = 3,
                TextAnswer = "1. К классу 4.1",
            });
            answers.Add(new Answer()
            {
                Id = 10,
                IsRight = true,
                QuestionId = 3,
                TextAnswer = "2. К классу 1.",
            });
            answers.Add(new Answer()
            {
                Id = 11,
                IsRight = false,
                QuestionId = 3,
                TextAnswer = "3. К классу 5.2.",
            });
            answers.Add(new Answer()
            {
                Id = 12,
                IsRight = false,
                QuestionId = 3,
                TextAnswer = "4. К классу 2.",
            });


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
            modelBuilder.Entity<Test>().HasData(tests);
            modelBuilder.Entity<Question>().HasData(questions);
            modelBuilder.Entity<Answer>().HasData(answers);




            base.OnModelCreating(modelBuilder);
        }
    }
}
