using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class QuestionItem
    {
        public string Question { get; set; }
        public List<AnswerItem> Answers { get; set; }
    }
}