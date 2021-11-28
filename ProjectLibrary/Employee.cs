using System;
using System.Collections.Generic;

#nullable disable

namespace ProjectLibrary
{
    public partial class Employee
    {
        public Employee()
        {
            ProjectEmployees = new HashSet<ProjectEmployee>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
        public string Department { get; set; }

        public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    }
}
