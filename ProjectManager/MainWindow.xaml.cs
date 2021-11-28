using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using ProjectLibrary;

namespace ProjectManager
{
  public partial class MainWindow : Window
  {
    public MainWindow() => InitializeComponent();

    private ProjectsContext db;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      db = new ProjectsContext();

      var cultureInfo = new System.Globalization.CultureInfo("en-US");
      cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
      System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
      /*
      Just First Time
      ReadCsvFiles();
      */

      GenerateButtons();
    }

    public void ReadCsvFiles()
    {
      string[] projects = File.ReadAllLines(@"csv/projects.csv");
      foreach (var projectString in projects)
      {
        Project project = new Project()
        {
          Name = projectString.Split(";")[1]
        };
        db.Projects.Add(project);
      }

      List<string> employees = File.ReadAllLines(@"csv/employees.csv").Skip(1).ToList();
      foreach (var employee in employees)
      {
        db.Employees.Add(Employee.Parse(employee));
      }

      db.SaveChanges();

      List<string> projectEmployees = File.ReadAllLines(@"csv/project_employees.csv").Skip(1).ToList();
      foreach (var projectEmployee in projectEmployees)
      {
        var splitedProjectEmployee = projectEmployee.Split(";");

        db.ProjectEmployees.Add(new ProjectEmployee()
        {
          AssingDate = DateTime.Now,
          EmployeeId = db.Employees
            .Where(x => x.Lastname + " " + x.Firstname == splitedProjectEmployee[0])
            .Select(x => x.Id)
            .ToList()[0],
          Employee = db.Employees
            .Where(x => x.Lastname + " " + x.Firstname == splitedProjectEmployee[0])
            .ToList()[0],
          Project = db.Projects.Where(x => x.Name == splitedProjectEmployee[1]).ToList()[0],
          ProjectId = db.Projects.Where(x => x.Name == splitedProjectEmployee[1]).Select(x => x.Id).ToList()[0]
        });
      }

      db.SaveChanges();


    }

    private void GenerateButtons()
    {
      foreach (var department in db.Employees.Select(x => x.Department).Distinct().ToList())
      {
        RadioButton radioButton = new RadioButton()
        {
          Content = department
        };
        radioButton.Click += ButtonsChanged;
        panDepartments.Children.Add(radioButton);
      }

      foreach (var department in db.Projects.Select(x => x.Name).ToList())
      {
        CheckBox checkBox = new CheckBox()
        {
          Content = department,
          IsChecked = true
        };
        checkBox.Click += ButtonsChanged;
        panProjects.Children.Add(checkBox);
      }
    }

    private void ButtonsChanged(object sender, RoutedEventArgs e)
    {

      string department = "";
      List<String> projects = new List<string>();

      foreach (var button in panDepartments.Children)
      {
        var radioButton = (RadioButton) button;
        if (radioButton.IsChecked is true)
        {
          department = radioButton.Content.ToString();
          break;
        }
      }

      foreach (var button in panProjects.Children)
      {
        var checkBox = (CheckBox) button;
        if (checkBox.IsChecked is true)
        {
          projects.Add(checkBox.Content.ToString());
        }
      }

      lstEmployees.ItemsSource = db.ProjectEmployees
        .Include(x => x.Employee)
        .Include(x => x.Project)
        .Where(x => x.Employee.Department == department)
        .Where(x => projects.Contains(x.Project.Name))
        .Distinct()
        .ToList()
        .Select(x => x.Employee);

      grdEmployees.ItemsSource = db.ProjectEmployees
        .Include(x => x.Employee)
        .Include(x => x.Project)
        .Where(x => x.Employee.Department == department)
        .Where(x => projects.Contains(x.Project.Name))
        .ToList();

    }


    private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
    {
      string salary = txtSalary.Text.Replace('.', ',');
      Employee employee = new Employee
      {
        Firstname = txtFirstname.Text,
        Lastname = txtLastname.Text,
        Age = Int32.Parse(txtAge.Text),
        Salary = Double.Parse(salary),
        Department = txtDepartment.Text
      };

      db.Employees.Add(employee);

      db.SaveChanges();

      int id = db.Employees.Select(x => x.Id).Max();

      string[] projectsSplitted = txtProjects.Text.Split(", ");

      foreach (var project in projectsSplitted)
      {
        ProjectEmployee projectEmployee = new ProjectEmployee()
        {
          EmployeeId = db.Employees.Where(x => x.Id == id).Select(x => x.Id).ToList()[0],
          Employee = db.Employees.Where(x => x.Id == id).Select(x => x).ToList()[0],
          ProjectId = db.Projects.Where(x => x.Name == project).Select(x => x.Id).ToList()[0],
          Project = db.Projects.Where(x => x.Name == project).Select(x => x).ToList()[0]
        };
        db.ProjectEmployees.Add(projectEmployee);
      }

      db.SaveChanges();

      ButtonsChanged(sender, e);

    }

    private void TxtDepartment_OnTextChanged(object sender, TextChangedEventArgs e)
    {

      if (db != null)
      {
        if (db.Employees.Select(x => x.Department).Distinct().ToList().Contains(txtDepartment.Text))
        {
          txtDepartment.Background = Brushes.White;
          btnAdd.IsEnabled = true;
        }
        else
        {
          txtDepartment.Background = Brushes.Red;
          btnAdd.IsEnabled = false;
        }
      }
    }

    private void TxtProjects_OnTextChanged(object sender, TextChangedEventArgs e)
    {
      if (db != null)
      {
        if (db.Projects.Select(x => x.Name).Distinct().ToList().Contains(txtProjects.Text))
        {
          txtProjects.Background = Brushes.White;
          btnAdd.IsEnabled = true;
        }
        else
        {
          txtProjects.Background = Brushes.Red;
          btnAdd.IsEnabled = false;
        }
      }
    }
  }
}