using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class questionmodel
    {

        public string Content { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
        public bool is_true { get; set; }
        public List<SubjectItem> SubjectItems { get; set; }
    }
}