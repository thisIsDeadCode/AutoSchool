using AutoSchool.Data;
using AutoSchool.Models.Interfaces;
using AutoSchool.Models.Tables;

namespace AutoSchool.Services
{
    public class HistoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public HistoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task SaveTohistory(User user, IModelToSaveInVisitHistory section)
        {
            var visit = new VisitHistory()
            {
                UserId = user.Id,
                Date = DateTime.Now,
            };

            if(section is Course)
            {
                visit.CourseId = section.Id;
            }
            else if (section is Theme)
            {
                visit.ThemeId = section.Id;
            }
            else if(section is Lecture)
            {
                visit.LectureId = section.Id;
            }
            else if (section is Test)
            {
                visit.TestId = section.Id;
            }

            await _dbContext.VisitHistories.AddAsync(visit);
            await _dbContext.SaveChangesAsync();
        }
    }
}
