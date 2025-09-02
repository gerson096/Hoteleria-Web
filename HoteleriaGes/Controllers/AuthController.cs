        // ...existing code...
using HoteleriaGes.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace HoteleriaGes.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        private readonly string connectionString = "server=localhost;database=hoteleriaweb;user=root;password=;";

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string correo, string contraseña)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Usuarios WHERE correo=@correo AND contraseña=@contraseña", conn);
                cmd.Parameters.AddWithValue("@correo", correo);
                cmd.Parameters.AddWithValue("@contraseña", contraseña);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                        // Autenticado
                        HttpContext.Session.SetString("Usuario", correo);
                        HttpContext.Session.SetString("Rol", reader["rol"].ToString());
                        return RedirectToAction("Index", "Home");
                }
                ViewBag.Error = "Correo o contraseña incorrectos";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

                try
                {
                    using (var conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        ViewBag.Error = "Conexión exitosa a la base de datos.";
                        var cmd = new MySqlCommand("INSERT INTO Usuarios (nombre, correo, contraseña, rol) VALUES (@nombre, @correo, @contraseña, @rol)", conn);
                        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("@contraseña", usuario.Contraseña);
                        cmd.Parameters.AddWithValue("@rol", usuario.Rol);
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            // Usuario guardado correctamente, redirigir al login
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ViewBag.Error = "No se pudo registrar el usuario. Verifica los datos.";
                            return View(usuario);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    ViewBag.Error = $"Error al registrar: {ex.Message}";
                    Console.WriteLine($"Error MySQL: {ex.Message}");
                    return View(usuario);
                }
        }
    }
}
