using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class QuestionViewModel
    {
        public Question Question { get; set; }
        public List<SubjectItem> SubjectItems { get; set; }
    }
}