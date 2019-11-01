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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.AttendanceTransactions = new HashSet<AttendanceTransaction>();
            this.Designation_History = new HashSet<Designation_History>();
            this.EmployeeLeaveAvaliabilities = new HashSet<EmployeeLeaveAvaliability>();
            this.LeaveApplications = new HashSet<LeaveApplication>();
            this.ReportingAuthorities = new HashSet<ReportingAuthority>();
            this.Users = new HashSet<User>();
        }
    
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public Nullable<long> Mobile { get; set; }
        public string Email { get; set; }
        public System.DateTime DOB { get; set; }
        public System.DateTime DOJ { get; set; }
        public Nullable<System.DateTime> DOQ { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public string IsManager { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttendanceTransaction> AttendanceTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Designation_History> Designation_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLeaveAvaliability> EmployeeLeaveAvaliabilities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeaveApplication> LeaveApplications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportingAuthority> ReportingAuthorities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
        public virtual Manager Manager { get; set; }
    }
}
