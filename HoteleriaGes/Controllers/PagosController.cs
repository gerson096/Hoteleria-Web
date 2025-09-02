using HoteleriaGes.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace HoteleriaGes.Controllers
{
    public class PagosController : Controller
    {
        private readonly string connectionString = "server=localhost;database=hoteleriaweb;user=root;password=;";

        [HttpGet]
        public IActionResult Realizar(int reservaId, decimal monto)
        {
            ViewBag.ReservaId = reservaId;
            ViewBag.Monto = monto;
            return View();
        }

        [HttpPost]
        public IActionResult Realizar(int reservaId, decimal monto, string metodo)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO Pagos (reserva_id, monto, metodo, fecha_pago) VALUES (@reserva_id, @monto, @metodo, @fecha_pago)", conn);
                cmd.Parameters.AddWithValue("@reserva_id", reservaId);
                cmd.Parameters.AddWithValue("@monto", monto);
                cmd.Parameters.AddWithValue("@metodo", metodo);
                cmd.Parameters.AddWithValue("@fecha_pago", DateTime.Now.Date);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
