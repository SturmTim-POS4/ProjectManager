using System;
using System.Collections.Generic;

#nullable disable

namespace ProjectLibrary
{
    public partial class ProjectEmployee
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public DateTime AssingDate { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Project Project { get; set; }
    }
}
