//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnCs.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Exam_Results
    {
        public int IdStudent { get; set; }
        public int IdExam { get; set; }
        public int IdQuestion { get; set; }
        public string Answer_student { get; set; }
    
        public virtual Exam Exam { get; set; }
        public virtual Question Question { get; set; }
        public virtual Student Student { get; set; }
    }
}
