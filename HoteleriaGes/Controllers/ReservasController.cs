using HoteleriaGes.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HoteleriaGes.Controllers
{
    public class ReservasController : Controller
    {
        private readonly string connectionString = "server=localhost;database=hoteleriaweb;user=root;password=;";

        [HttpGet]
        public IActionResult Crear()
        {
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "cliente")
            {
                return RedirectToAction("Login", "Auth");
            }
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

    [HttpPost]
    public IActionResult Seleccionar(int habitacionId, int dias)
        {
            // Obtener el usuario actual (correo)
            var correo = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(correo))
                return RedirectToAction("Login", "Auth");

            int clienteId = 0;
            int reservaId = 0;
            decimal monto = 0;
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // Buscar el cliente por correo
                var cmdCliente = new MySqlCommand("SELECT id FROM Clientes WHERE correo=@correo", conn);
                cmdCliente.Parameters.AddWithValue("@correo", correo);
                var reader = cmdCliente.ExecuteReader();
                if (reader.Read())
                {
                    clienteId = reader.GetInt32("id");
                }
                reader.Close();

                if (clienteId == 0)
                {
                    // Si no existe, crear el cliente
                    var cmdInsert = new MySqlCommand("INSERT INTO Clientes (nombre, correo) VALUES (@nombre, @correo); SELECT LAST_INSERT_ID();", conn);
                    cmdInsert.Parameters.AddWithValue("@nombre", correo);
                    cmdInsert.Parameters.AddWithValue("@correo", correo);
                    clienteId = Convert.ToInt32(cmdInsert.ExecuteScalar());
                }

                // Obtener el precio de la habitación
                var cmdPrecio = new MySqlCommand("SELECT precio FROM Habitaciones WHERE id=@id", conn);
                cmdPrecio.Parameters.AddWithValue("@id", habitacionId);
                monto = Convert.ToDecimal(cmdPrecio.ExecuteScalar());

                // Crear la reserva
                var cmdReserva = new MySqlCommand("INSERT INTO Reservas (cliente_id, habitacion_id, fecha_entrada, fecha_salida, estado) VALUES (@cliente_id, @habitacion_id, @fecha_entrada, @fecha_salida, @estado); SELECT LAST_INSERT_ID();", conn);
                cmdReserva.Parameters.AddWithValue("@cliente_id", clienteId);
                cmdReserva.Parameters.AddWithValue("@habitacion_id", habitacionId);
                cmdReserva.Parameters.AddWithValue("@fecha_entrada", DateTime.Now.Date);
                cmdReserva.Parameters.AddWithValue("@fecha_salida", DateTime.Now.Date.AddDays(dias));
                cmdReserva.Parameters.AddWithValue("@estado", "activa");
                reservaId = Convert.ToInt32(cmdReserva.ExecuteScalar());

                // Actualizar estado de la habitación
                var cmdUpdate = new MySqlCommand("UPDATE Habitaciones SET estado='ocupada' WHERE id=@id", conn);
                cmdUpdate.Parameters.AddWithValue("@id", habitacionId);
                cmdUpdate.ExecuteNonQuery();
            }
            // Redirigir a la página de pago
            return RedirectToAction("Realizar", "Pagos", new { reservaId = reservaId, monto = monto });
        }
    }
}
