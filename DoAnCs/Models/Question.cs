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
    
    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            this.Exam_Results = new HashSet<Exam_Results>();
        }
    
        public int IdQuestion { get; set; }
        public string Contentt { get; set; }
        public string optionA { get; set; }
        public string optionB { get; set; }
        public string optionC { get; set; }
        public string optionD { get; set; }
        public Nullable<int> IdExam { get; set; }
        public Nullable<int> IdDifficulty { get; set; }
        public Nullable<int> IdDapan { get; set; }
        public string DapAn { get; set; }
    
        public virtual Answer Answer { get; set; }
        public virtual Difficulty Difficulty { get; set; }
        public virtual Exam Exam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exam_Results> Exam_Results { get; set; }
    }
}
