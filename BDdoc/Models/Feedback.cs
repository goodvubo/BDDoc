//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BDdoc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Feedback
    {
        public int feedback_ID { get; set; }
        public Nullable<int> ID { get; set; }
        public string comment { get; set; }
        public string Popularity { get; set; }
        public string Reason { get; set; }
        public string patient_Name { get; set; }
        public Nullable<int> patient_Mobile { get; set; }
        public string patient_email { get; set; }
        public string time { get; set; }
        public string Anonymous_Comment { get; set; }
    
        public virtual Doctor Doctor { get; set; }
    }
}
