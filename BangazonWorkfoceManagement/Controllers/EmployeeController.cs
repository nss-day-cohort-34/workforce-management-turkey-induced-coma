using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkfoceManagement.Models;
using BangazonWorkfoceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkfoceManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSupervisor, d.Name, d.Id AS TheDepartmentId
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        ON e.DepartmentId = d.Id
                                        ORDER BY d.Name, e.IsSupervisor, e.LastName, e.FirstName";
                    var reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {

                        Employee employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheDepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employee/Details/5
        public ActionResult Details(int id)
        {
            var employee = GetEmployeeWithComputerTraining(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // GET: Employee/AssignTraining
        public ActionResult AssignTraining(int id)
        {
            var viewModel = new AssignTrainingViewModel()
            {
                Employee = GetEmployeeById(id),
                AllTrainingPrograms = GetEmployeeTrainingById(id)
            };
            return View(viewModel);
        }

        // POST: Employee/AssignTraining
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTraining(AssignTrainingViewModel TrainingViewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO EmployeeTraining(EmployeeId, TrainingProgramId)
                                                VALUES (@EmployeeId, @TrainingProgramId);";
                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", TrainingViewModel.Employee.Id));
                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", TrainingViewModel.TrainingProgram.Id));

                        cmd.ExecuteNonQuery();
                    }

                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                return View();
            }
        }


        // GET: Employee/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel()
            {
                Departments = GetDepartments()
            };
            return View(viewModel);
        }




        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel viewModel)
        {
            try
            {
                var newEmployee = viewModel.Employee;

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO Employee(FirstName, LastName, DepartmentId, IsSupervisor)
                                                VALUES (@firstName, @lastName, @departmentId, @isSupervisor);";
                        cmd.Parameters.Add(new SqlParameter("@firstName", newEmployee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", newEmployee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", newEmployee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", newEmployee.IsSupervisor));

                        cmd.ExecuteNonQuery();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new EmployeeEditViewModel()
            {
                Employee = GetEmployeeById(id),
                Departments = GetDepartments(),
                CurrentComputer = GetEmployeesComputer(id),
                UnassignedComputers = GetUnassignedComputers()
            };
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        {
            var updatedEmployee = viewModel.Employee;
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (viewModel.Employee.ComputerId == 0)
                        {
                            cmd.CommandText = @"UPDATE Employee
                                                SET FirstName = @firstName,
                                                LastName = @lastName,
                                                DepartmentId = @departmentId
                                                WHERE Id = @id;
                                                ";
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@firstName", updatedEmployee.FirstName));
                            cmd.Parameters.Add(new SqlParameter("@lastName", updatedEmployee.LastName));
                            cmd.Parameters.Add(new SqlParameter("@departmentId", updatedEmployee.DepartmentId));

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception("No rows affected");
                        }
                        else
                        {
                            cmd.CommandText = @"UPDATE ComputerEmployee
                                                SET UnassignDate = @assignDate
                                                WHERE EmployeeId = @id AND UnassignDate IS NULL;
                                                INSERT INTO ComputerEmployee
                                                (EmployeeId, ComputerId, AssignDate)
                                                VALUES (@id, @computerId, @assignDate);
                                                UPDATE Employee
                                                SET FirstName = @firstName,
                                                LastName = @lastName,
                                                DepartmentId = @departmentId
                                                WHERE Id = @id;";
                            cmd.Parameters.Add(new SqlParameter("@id", id));
                            cmd.Parameters.Add(new SqlParameter("@firstName", updatedEmployee.FirstName));
                            cmd.Parameters.Add(new SqlParameter("@lastName", updatedEmployee.LastName));
                            cmd.Parameters.Add(new SqlParameter("@departmentId", updatedEmployee.DepartmentId));
                            cmd.Parameters.Add(new SqlParameter("@computerId", updatedEmployee.ComputerId));
                            cmd.Parameters.Add(new SqlParameter("@assignDate", DateTime.Now));

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception("No rows affected");
                        }
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, d.Name, d.Id AS TheDepartmentId
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        ON e.DepartmentId = d.Id
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheDepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                    }
                    reader.Close();
                    return employee;
                }
            }
        }
        private Employee GetEmployeeWithComputerTraining(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSupervisor, d.Name, d.Id AS TheDepartmentId, c.Id AS ComputerId, c.Make, c.Manufacturer, ce.AssignDate, c.PurchaseDate
                                        FROM Employee e
                                        LEFT JOIN Department d
                                        ON e.DepartmentId = d.Id
                                        LEFT JOIN ComputerEmployee ce
                                        ON e.Id = ce.EmployeeId
                                        LEFT JOIN Computer c
                                        ON c.Id = ce.ComputerId
                                        WHERE e.Id = @id AND ce.UnassignDate IS NULL";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();
                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheDepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            },
                            AssignedComputer = new Computer()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                AssignDate = reader.GetDateTime(reader.GetOrdinal("AssignDate")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                            }
                        };
                    }
                    reader.Close();
                    employee.AllTrainingPrograms = GetEmployeeTrainingPrograms(id, cmd);
                    return employee;
                }
            }
        }

        private List<TrainingProgram> GetEmployeeTrainingById(int EmployeeId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        FROM TrainingProgram tp WHERE StartDate > GETDATE()
                                        LEFT JOIN EmployeeTraining et on et.id =";
                    var reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();
                    return (trainingPrograms);
                }
            }
        }
        private List<TrainingProgram> GetEmployeeTrainingPrograms(int employeeId, SqlCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = @"SELECT t.Id, t.Name, t.StartDate, t.EndDate
                                FROM Employee e
                                LEFT JOIN EmployeeTraining et
                                ON e.Id = et.EmployeeId
                                LEFT JOIN TrainingProgram t
                                ON t.Id = et.TrainingProgramId
                                WHERE e.Id = @id
                                ORDER BY t.StartDate DESC";
            cmd.Parameters.Add(new SqlParameter("@id", employeeId));
            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TrainingProgram trainingProgram = new TrainingProgram()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                };
                trainingPrograms.Add(trainingProgram);
            }
            reader.Close();
            return trainingPrograms;
        }
        private List<Department> GetDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                        FROM Department
                                        ORDER BY Name";
                    var reader = cmd.ExecuteReader();
                    List<Department> deparments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        deparments.Add(department);
                    }
                    reader.Close();
                    return deparments;
                }
            }
        }
        private Computer GetEmployeesComputer(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, c.Id AS TheComputerId, c.Make, c.Manufacturer
                                        FROM Employee e
                                        LEFT JOIN ComputerEmployee ce
                                        ON e.Id = ce.EmployeeId
                                        LEFT JOIN Computer c
                                        ON c.Id = ce.ComputerId
                                        WHERE EmployeeId = @id AND UnassignDate IS NULL
                                        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();
                    Computer computer = null;
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("TheComputerId")))
                        {
                            computer = new Computer()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TheComputerId")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                        }
                    }
                    reader.Close();
                    return computer;
                }
            }
        }
        private List<Computer> GetUnassignedComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Make, c.Manufacturer
                                        FROM Computer c
                                        LEFT JOIN ComputerEmployee ce
                                        ON c.Id = ce.ComputerId
                                        LEFT JOIN Employee e
                                        ON e.Id = ce.EmployeeId
                                        WHERE (ce.UnassignDate IS NOT NULL OR ce.AssignDate IS NULL) AND c.DecomissionDate IS NULL;";

                    var reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        Computer computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                        };
                        computers.Add(computer);
                    }
                    reader.Close();
                    return computers;
                }
            }
        }
    }
}

