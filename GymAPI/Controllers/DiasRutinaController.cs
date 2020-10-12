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
    public class DiasRutinaController : ControllerBase, IReactAdminController<DiasRutina>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiasRutina>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM DiasRutina";
            var t = new DiasRutina();

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

            List<DiasRutina> diasRutinas = new List<DiasRutina>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                DiasRutina diasRutina;
                while (reader.Read())
                {
                    diasRutina = new DiasRutina();
                    diasRutina.ID = int.Parse(reader[0].ToString());
                    diasRutina.IDRutina = int.Parse(reader[1].ToString());
                    diasRutina.IDDia = int.Parse(reader[2].ToString());
                    diasRutina.IDEjercicio = int.Parse(reader[3].ToString());
                    diasRutina.Repeticiones = int.Parse(reader[4].ToString());
                    diasRutina.Series = int.Parse(reader[5].ToString());
                    diasRutinas.Add(diasRutina);
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

            var count = diasRutinas.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(DiasRutina).Name.ToLower()} {from}-{to}/{count}");
            return diasRutinas;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiasRutina>> Get(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM DiasRutina WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            DiasRutina diaRutina = new DiasRutina();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    diaRutina.ID = int.Parse(reader[0].ToString());
                    diaRutina.IDDia = int.Parse(reader[1].ToString());
                    diaRutina.IDEjercicio = int.Parse(reader[2].ToString());
                    diaRutina.Repeticiones = int.Parse(reader[3].ToString());
                    diaRutina.Series = int.Parse(reader[4].ToString());
                    connection.Close();
                    return diaRutina;
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
        public async Task<ActionResult<DiasRutina>> Post(DiasRutina diasRutina)
        {

            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO DiasRutina (IDRutina, IDDia, IDEjercicio, Repeticiones, Series) " +
                $"VALUES(" +
                $"{diasRutina.IDRutina}, " +
                $"{diasRutina.IDDia}, " +
                $"{diasRutina.IDEjercicio}, " +
                $"{diasRutina.Repeticiones}, " +
                $"{diasRutina.Series}" +
                $");";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, diasRutina);
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
        public async Task<IActionResult> Put(int ID, DiasRutina diasRutina)
        {
            var entityId = (int)typeof(DiasRutina).GetProperty("ID").GetValue(diasRutina);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE DiasRutina SET " +
                $"IDRutina = {diasRutina.IDRutina}, " +
                $"IDDia = {diasRutina.IDDia}, " +
                $"IDEjercicio = {diasRutina.IDEjercicio}, " +
                $"Repeticiones = {diasRutina.Repeticiones}, " +
                $"Series = {diasRutina.Series} " +
                $"WHERE ID = {diasRutina.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, diasRutina);
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
        public async Task<ActionResult<DiasRutina>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM DiasRutina WHERE ID = {ID};";
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
