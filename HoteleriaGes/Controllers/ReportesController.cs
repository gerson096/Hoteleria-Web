using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace HoteleriaGes.Controllers
{
    public class ReportesController : Controller
    {
        private readonly string connectionString = "server=localhost;database=hoteleriaweb;user=root;password=;";

        public IActionResult Ocupacion()
        {
            var ocupaciones = new List<dynamic>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT h.numero, h.tipo, r.fecha_entrada, r.fecha_salida, DATEDIFF(r.fecha_salida, r.fecha_entrada) AS dias
                                            FROM Habitaciones h
                                            JOIN Reservas r ON h.id = r.habitacion_id
                                            WHERE r.estado = 'activa'", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ocupaciones.Add(new
                    {
                        Numero = reader.GetString("numero"),
                        Tipo = reader.GetString("tipo"),
                        FechaEntrada = reader.GetDateTime("fecha_entrada"),
                        FechaSalida = reader.GetDateTime("fecha_salida"),
                        Dias = reader.GetInt32("dias")
                    });
                }
            }
            return View(ocupaciones);
        }
    }
}
