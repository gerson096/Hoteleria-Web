using HoteleriaGes.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HoteleriaGes.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly string connectionString = "server=localhost;database=hoteleriaweb;user=root;password=;";

        public IActionResult Disponibles()
        {
            var habitaciones = new List<Habitacion>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Habitaciones WHERE estado='disponible'", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    habitaciones.Add(new Habitacion
                    {
                        Id = reader.GetInt32("id"),
                        Numero = reader.GetString("numero"),
                        Tipo = reader.GetString("tipo"),
                        Precio = reader.GetDecimal("precio"),
                        Estado = reader.GetString("estado")
                    });
                }
            }
            return View(habitaciones);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "admin")
            {
                return RedirectToAction("Disponibles");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Habitacion habitacion)
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "admin")
            {
                return RedirectToAction("Disponibles");
            }
            if (!ModelState.IsValid)
                return View(habitacion);

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO Habitaciones (numero, tipo, precio, estado) VALUES (@numero, @tipo, @precio, @estado)", conn);
                cmd.Parameters.AddWithValue("@numero", habitacion.Numero);
                cmd.Parameters.AddWithValue("@tipo", habitacion.Tipo);
                cmd.Parameters.AddWithValue("@precio", habitacion.Precio);
                cmd.Parameters.AddWithValue("@estado", habitacion.Estado);
                try
                {
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                        return RedirectToAction("Disponibles");
                    else
                    {
                        ViewBag.Error = "No se pudo guardar la habitación.";
                        return View(habitacion);
                    }
                }
                catch (MySqlException ex)
                {
                    ViewBag.Error = $"Error: {ex.Message}";
                    return View(habitacion);
                }
            }
        }
    }
}
