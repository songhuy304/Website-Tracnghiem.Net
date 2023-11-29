using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class QuestionAnswer
    {
        public int QuestionNumber { get; set; }
        public string QuestionContent { get; set; }
        public List<string> Choices { get; set; }
      
        public int CorrectAnswerIndex { get; set; }
        public int UserAnswerIndex { get; set; }
        public int Score { get; set; }
    }

}