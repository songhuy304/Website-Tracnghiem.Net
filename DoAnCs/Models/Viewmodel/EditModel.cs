using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCs.Models.Viewmodel
{
    public class EditModel
    {
        public string Contentt { get; set; }
        public List<SubjectItem> subjectItems { get; set; }
    }
}