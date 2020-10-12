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
    public class DireccionController : ControllerBase, IReactAdminController<Direccion>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Direccion>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Direccion";
            var t = new Direccion();

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

            List<Direccion> direcciones = new List<Direccion>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                Direccion direccion;
                while (reader.Read())
                {
                    direccion = new Direccion();
                    direccion.ID = int.Parse(reader[0].ToString());
                    direccion.IDSector = int.Parse(reader[1].ToString());
                    direccion.Dir = reader[2].ToString();

                    direcciones.Add(direccion);
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

            var count = direcciones.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(Direccion).Name.ToLower()} {from}-{to}/{count}");
            return direcciones;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Direccion>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Direccion WHERE ID = {id};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            Direccion direccion= new Direccion();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    direccion.ID = int.Parse(reader[0].ToString());
                    direccion.IDSector = int.Parse(reader[1].ToString());
                    direccion.Dir = reader[2].ToString();
                    connection.Close();
                    return direccion;
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
        public async Task<ActionResult<Direccion>> Post(Direccion direccion)
        {

            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO DIRECCION " +
                $"VALUES(" +
                $"{direccion.IDSector}, " +
                $"{direccion.Dir}" +
                $");";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, direccion);
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
        public async Task<IActionResult> Put(int ID, Direccion direccion)
        {
            var entityId = (int)typeof(Direccion).GetProperty("ID").GetValue(direccion);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE DIRECCION SET " +
                $"IDSector = '{direccion.IDSector}', " +
                $"Dir = '{direccion.Dir}' " +
                $"WHERE ID = {direccion.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, direccion);
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
        public async Task<ActionResult<Direccion>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM Direccion WHERE ID = {ID};";
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