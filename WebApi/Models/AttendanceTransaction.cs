//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttendanceTransaction
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public System.DateTime Checkin { get; set; }
        public System.DateTime Checkout { get; set; }
        public string Status { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
