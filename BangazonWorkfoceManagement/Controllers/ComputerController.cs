using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkfoceManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkfoceManagement.Controllers
{
    public class ComputerController : Controller
    {
        private readonly IConfiguration _config;

        public ComputerController(IConfiguration config)
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

        // GET: Computer
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM Computer c";

                    var reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();


                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computers.Add(
                                new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                                });

                        }
                        else
                        {
                            computers.Add(
                                new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                    DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                                });

                        }
                    }

                    reader.Close();
                    return View(computers);
                }
            }
        }

        // GET: Computer/Details/5
        public ActionResult Details(int id)
        {
            Computer computer = GetComputerById(id);
            return View(computer);
        }

        // GET: Computer/Create
        public ActionResult Create()
        {
            var viewModel = new Computer();
            return View(viewModel);
        }

        // POST: Computer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Computer computer)
        {
            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @" INSERT INTO Computer (PurchaseDate, Make, Manufacturer)
                                             VALUES (@PurchaseDate, @Make, @Manufacturer);";
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));


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

        // GET: Computer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computer/Edit/5
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

        // GET: Computer/Delete/5
        public ActionResult Delete(int id)
        {
            var computer = GetComputerById(id);
            return View(computer);
        }

        // POST: Computer/Delete/5
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
                        cmd.CommandText = @"SELECT c.Id, c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate, ce.AssignDate
                                            FROM Computer c
                                            LEFT JOIN ComputerEmployee ce
                                            ON c.Id = ce.ComputerId
                                            LEFT JOIN Employee e
                                            ON e.Id = ce.EmployeeId
                                            WHERE ce.AssignDate IS NULL AND c.Id = @Id;";

                        cmd.Parameters.Add(new SqlParameter("@Id", id));
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {

                            var computerId = reader.GetInt32(reader.GetOrdinal("Id"));

                            if (id == computerId)

                            {
                                reader.Close();
                                cmd.CommandText = @"DELETE FROM Computer WHERE Id = @Id";
                                cmd.ExecuteNonQuery();
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                throw new Exception("Computer cannot be deleted.");
                            }
                        }
                        return RedirectToAction(nameof(Index));

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //GET: /Computer/Filter 
        public IActionResult Filter(string searchString)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Make, c.Manufacturer, c.PurchaseDate, c.DecomissionDate
                                        FROM Computer c
                                        WHERE c.Make LIKE @searchString OR c.Manufacturer LIKE @searchString";
                    cmd.Parameters.Add(new SqlParameter("@searchString", $"%{searchString}%"));

                    var reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computers.Add(
                                new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                                });

                        }
                        else
                        {
                            computers.Add(
                                new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                    DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                                });

                        }
                    }
                    reader.Close();
                    return View(computers);
                }
            }
        }


        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Manufacturer
                                        FROM Computer c
                                        WHERE @Id = Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Computer computer = null;


                    if (reader.Read())
                    {
                        if (reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer = new Computer

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),

                            };

                        }
                        else
                        {

                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
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


    }
}