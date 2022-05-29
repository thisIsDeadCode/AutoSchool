﻿using AutoSchool.Models.Interfaces;

namespace AutoSchool.Models.Tables
{
    public class Test : IModelToSaveInVisitHistory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AmountQuestions { get; set; }
        public string? Description { get; set; }

        public double? LastResult { get; set; }
        public DateTime? Date { get; set; }

        
        public Theme Theme { get; set; }

        public IEnumerable<Question> Questions { get; set; }
        public IEnumerable<ResultTest> ResultTests { get; set; }
        public IEnumerable<VisitHistory> Visits { get; set; }
    }
}
