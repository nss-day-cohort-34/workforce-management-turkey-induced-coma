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
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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
        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT D.Id, D.Name, D.Budget, COUNT(E.Id) AS EmployeeCount
                                        FROM Department D
                                        JOIN Employee E ON D.ID = E.DepartmentID
                                        GROUP BY D.Id, D.Name, D.Budget";
                    var reader = cmd.ExecuteReader();
                    List<DepartmentListViewModel> departments = new List<DepartmentListViewModel>();

                    while (reader.Read())
                    {
                        DepartmentListViewModel department = new DepartmentListViewModel()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))
                        };

                        departments.Add(department);
                    };

                    reader.Close();
                    return View(departments);
                }
            }
        }
        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            Department department = GetDepartmentById(id);
            return View(department);
        }
        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
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
        private Department GetDepartmentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT D.Id, D.Name, D.Budget, 
                                        E.Id AS EmployeeId, E.FirstName, E.LastName, E.IsSupervisor
                                        FROM Department D
                                        LEFT JOIN Employee E 
                                        ON D.ID = E.DepartmentID
                                        WHERE D.ID  = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Department department = null;
                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            department.Employees.Add(
                                new Employee()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("Id"))
                                }
                            );
                        }
                    }
                    reader.Close();
                    return department;
                }
            }
        }
    }
}


//Employee employee = new Employee()
//{
  //Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
   //FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
   // LastName = reader.GetString(reader.GetOrdinal("LastName")),
    //IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
    //DepartmentId = reader.GetInt32(reader.GetOrdinal("Id"))
//};
//department.Employees.Add(employee);