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

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanMembresiaController : ControllerBase, IReactAdminController<PlanMembresia>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanMembresia>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM PlanMembresia";
            var t = new PlanMembresia();

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

            List<PlanMembresia> planMembresias = new List<PlanMembresia>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                PlanMembresia planMembresia;
                while (reader.Read())
                {
                    planMembresia = new PlanMembresia();
                    planMembresia.ID = int.Parse(reader[0].ToString());
                    planMembresia.Nombre = reader[1].ToString();
                    planMembresia.Descripcion = reader[2].ToString();
                    planMembresia.TiempoValidez = int.Parse(reader[3].ToString());
                    planMembresia.Monto = float.Parse(reader[4].ToString());
                    planMembresia.Estado = Convert.ToBoolean(reader[5].ToString());
                    planMembresias.Add(planMembresia);
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

            var count = planMembresias.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(PlanMembresia).Name.ToLower()} {from}-{to}/{count}");
            return planMembresias;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanMembresia>> Get(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM PlanMembresia WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            PlanMembresia planMembresia = new PlanMembresia();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    planMembresia.ID = int.Parse(reader[0].ToString());
                    planMembresia.Nombre = reader[1].ToString();
                    planMembresia.Descripcion = reader[2].ToString();
                    planMembresia.TiempoValidez = int.Parse(reader[3].ToString());
                    planMembresia.Monto = float.Parse(reader[4].ToString());
                    planMembresia.Estado = Convert.ToBoolean(reader[5].ToString());
                    connection.Close();
                    return planMembresia;
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
        public async Task<ActionResult<PlanMembresia>> Post(PlanMembresia planMembresia)
        {

            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO PlanMembresia VALUES(" +
                $"'{planMembresia.Nombre}', " +
                $"'{planMembresia.Descripcion}', " +
                $"{planMembresia.TiempoValidez}, " +
                $"{planMembresia.Monto}, " +
                $"{planMembresia.Estado});";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, planMembresia);
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
        public async Task<IActionResult> Put(int ID, PlanMembresia planMembresia)
        {
            var entityId = (int)typeof(PlanMembresia).GetProperty("ID").GetValue(planMembresia);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE PlanMembresia SET " +
                $"Nombre = '{planMembresia.Nombre}', " +
                $"Descripcion = '{planMembresia.Descripcion}'," +
                $"TiempoValidez = {planMembresia.TiempoValidez}, " +
                $"Monto = {planMembresia.Monto}, " +
                $"Estado = {planMembresia.Estado} " +
                $"WHERE ID = {planMembresia.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, planMembresia);
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
        public async Task<ActionResult<PlanMembresia>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM PlanMembresia WHERE ID = {ID};";
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
