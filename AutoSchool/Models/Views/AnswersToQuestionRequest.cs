﻿namespace AutoSchool.Models.Views
{
    public class AnswersToQuestionRequest 
    {
        public long QuestionId { get; set; }
        public string QuestionText { get; set; }

        public List<AnswerRequest> Answers { get; set; }
    }
}
