﻿using System;
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
    public class TrainingProgramController : Controller
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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
        // GET: TrainingProgram
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        FROM TrainingProgram tp WHERE StartDate > GETDATE()";
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
                    return View(trainingPrograms);
                }
            }
        }

        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        {
            TrainingProgram program = GetTrainingById(id);
            return View(program);
        }

        //GET: TrainingProgram/PastPrograms
        public ActionResult PastPrograms()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        FROM TrainingProgram tp WHERE StartDate < GETDATE()";
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
                    return View(trainingPrograms);
                }
            }
        }
        // GET: TrainingProgram/PastPrograms/Details/5
        public ActionResult PastDetails(int id)
        {
            PastTrainingDetailsViewModel viewModel = new PastTrainingDetailsViewModel()
            {
                TrainingProgram = GetTrainingById(id),
                Attendees = GetAttendees(id)
            };
            return View(viewModel);
        }

        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram TrainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO TrainingProgram(Name, StartDate, EndDate, MaxAttendees)
                                                VALUES (@name, @startDate, @endDate, @maxAttendees);";
                        cmd.Parameters.Add(new SqlParameter("@Name", TrainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", TrainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", TrainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", TrainingProgram.MaxAttendees));

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

        
        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Edit/5
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

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            var Training = GetTrainingById(id);
            return View(Training);
        }

        // POST: TrainingProgram/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM EmployeeTraining WHERE TrainingProgramId = @id;
                            DELETE FROM TrainingProgram WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private TrainingProgram GetTrainingById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT E.FirstName, E.LastName, T.Name, T.StartDate, T.EndDate, T.MaxAttendees, E.Id, ET.EmployeeId
FROM TrainingProgram T
LEFT JOIN EmployeeTraining ET ON T.Id = ET.TrainingProgramId
LEFT JOIN Employee E ON ET.EmployeeId = E.Id
WHERE T.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram trainingProgram = null;
                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {

                            trainingProgram = new TrainingProgram
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }


                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                            trainingProgram.Employees.Add(
                                new Employee()
                                {
                                    Id = employeeId,
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }
                            );

                        }
                     

                    }

                    reader.Close();
                    return trainingProgram;
                }
            }
        }

        private List<Employee> GetAttendees(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName
                                        FROM Employee e
                                        LEFT JOIN EmployeeTraining et
                                        ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram t
                                        ON t.Id = et.TrainingProgramId
                                        WHERE t.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        employees.Add(new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                        });

                    }

                    reader.Close();
                    return employees;
                }
            }
        }
    }
}
