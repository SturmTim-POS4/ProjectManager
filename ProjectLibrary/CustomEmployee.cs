using System;
using System.Linq;

namespace ProjectLibrary
{
    public partial class Employee
    {
        public override string ToString()
        {
            return $"{Firstname} {Lastname} ({Department}) [{string.Join(", ",ProjectEmployees.Select(x => x.Project.Name).ToList())}]";
        }

        public static Employee Parse(string employeeString)
        {
            string[] splitedEmployee = employeeString.Split(";");
            splitedEmployee[4] = splitedEmployee[4].Replace('.', ',');
            return new Employee()
            {
                Firstname = splitedEmployee[1],
                Lastname = splitedEmployee[2],
                Age = Int32.Parse(splitedEmployee[3]),
                Salary = Double.Parse(splitedEmployee[4]),
                Department = splitedEmployee[5]
            };
        }

        public string Name
        {
            get { return $"{Firstname} {Lastname}"; }
        }
    }
}