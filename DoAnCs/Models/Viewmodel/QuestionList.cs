using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class QuestionList
    {
        public List<Question> Questions { get; set; }
        public List<SubjectItem> SubjectItems { get; set; }
    }
}