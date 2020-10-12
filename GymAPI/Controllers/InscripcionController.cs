using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GymAPI.Config;
using GymAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionController : ControllerBase, IReactAdminController<Inscripcion>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inscripcion>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Inscripcion";
            var t = new Inscripcion();

            if (!string.IsNullOrEmpty(filter))
            {
                var filterVal = (JObject)JsonConvert.DeserializeObject(filter);
                int i = 0;
                foreach (var f in filterVal)
                {
                    var valueArr = f.Value.ToArray();
                    var valueString = valueArr[0].Value<string>();
                    if (t.GetType().GetProperty(f.Key.ToUpper()).PropertyType == typeof(string))
                    {
                        if (i == 0)
                        {
                            sql += $" where {f.Key.ToUpper()} = '{valueString}'";
                        }
                        else
                        {
                            sql += $" OR where {f.Key.ToUpper()} = '{valueString}'";
                        }

                    }
                    else
                    {
                        if (i == 0)
                        {
                            sql += $" where {f.Key.ToUpper()} = {valueString}";
                        }

                        else
                        {
                            sql += $" OR where {f.Key.ToUpper()} = {valueString}";
                        }

                    }
                    i += 1;
                }
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortVal = JsonConvert.DeserializeObject<List<string>>(sort);
                var condition = sortVal.First();
                var order = sortVal.Last() == "ASC" ? "" : "DESC";
                sql += $" ORDER BY {condition} {order}";
            }

            var from = 0;
            var to = 0;
            if (!string.IsNullOrEmpty(range))
            {
                var rangeVal = JsonConvert.DeserializeObject<List<int>>(range);
                from = rangeVal.First();
                to = rangeVal.Last();
                sql += $" OFFSET {from} ROWS FETCH NEXT {to - from + 1} ROWS ONLY";
            }

            sql += ";";

            Console.WriteLine("Last SQL", sql);
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            connection.Open();

            List<Inscripcion> inscripcions = new List<Inscripcion>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                Inscripcion inscripcion;
                while (reader.Read())
                {
                    inscripcion = new Inscripcion();
                    inscripcion.ID = int.Parse(reader[0].ToString());
                    inscripcion.IDPlan = int.Parse(reader[1].ToString());
                    inscripcion.FPago = Convert.ToDateTime(reader[2].ToString());
                    inscripcion.FExpiracion = Convert.ToDateTime(reader[3].ToString());
                    inscripcion.AutoRenovacion = Convert.ToBoolean(reader[4].ToString());

                    inscripcions.Add(inscripcion);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }

            var count = inscripcions.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(Inscripcion).Name.ToLower()} {from}-{to}/{count}");
           return inscripcions;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscripcion>> Get(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Inscripcion WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            Inscripcion inscripcion = new Inscripcion();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    inscripcion = new Inscripcion();
                    inscripcion.ID = int.Parse(reader[0].ToString());
                    inscripcion.IDPlan = int.Parse(reader[1].ToString());
                    inscripcion.FPago = Convert.ToDateTime(reader[2].ToString());
                    inscripcion.FExpiracion = Convert.ToDateTime(reader[3].ToString());
                    inscripcion.AutoRenovacion = Convert.ToBoolean(reader[4].ToString());
                    connection.Close();
                    return inscripcion;
                }
                else
                {
                    connection.Close();
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<AdminController>
        [HttpPost]
        public async Task<ActionResult<Inscripcion>> Post(Inscripcion inscripcion)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO Inscripcion " +
                $"VALUES(" +
                $"{inscripcion.IDPlan}, " +
                $"'{inscripcion.FPago}', " +
                $"'{inscripcion.FExpiracion}', " +
                $"{inscripcion.AutoRenovacion});";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, inscripcion);
                else
                    return StatusCode(501, "No se pudo registrar");
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int ID, Inscripcion inscripcion)
        {
            var entityId = (int)typeof(Inscripcion).GetProperty("ID").GetValue(inscripcion);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE Inscripcion SET " +
                $"IDPlan = {inscripcion.IDPlan}, " +
                $"FPago = '{inscripcion.FPago}', " +
                $"FExpiracion = '{inscripcion.FExpiracion}', " +
                $"AutoRenovacion = {inscripcion.AutoRenovacion} " +
                $"WHERE ID = {inscripcion.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, inscripcion);
                else
                    return StatusCode(501, "No se pudo modificar");
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Inscripcion>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM Inscripcion WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, "Se elimino correctamente");
                else
                    return StatusCode(501, "No se pudo eliminar");
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }
    }
}