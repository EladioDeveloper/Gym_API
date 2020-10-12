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
    public class EvaluacionMensualController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvaluacionMensual>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM EvaluacionMensual";
            var t = new Cliente();

            if (!string.IsNullOrEmpty(filter))
            {
                var filterVal = (JObject)JsonConvert.DeserializeObject(filter);
                int i = 0;
                foreach (var f in filterVal)
                {
                    if (t.GetType().GetProperty(f.Key).PropertyType == typeof(string))
                    {
                        if (i == 0) { 
                            sql += $" where {f.Key} == '{f.Value}'";
                        }
                        else {
                            sql += $" OR where {f.Key} == '{f.Value}'";
                        }
                            
                    }
                    else
                    {
                        if (i == 0) {
                            sql += $" where {f.Key} == {f.Value}";
                        }

                        else {
                            sql += $" OR where {f.Key} == {f.Value}";
                        }
                            
                    }
                    i += 1;
                }
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortVal = JsonConvert.DeserializeObject<List<string>>(sort);
                var condition = sortVal.First();
                var order = sortVal.Last() == "ASC" ? "" : "descending";
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

            List<EvaluacionMensual> evaluacionMensuals = new List<EvaluacionMensual>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                EvaluacionMensual evaluacionMensual;
                while (reader.Read())
                {
                    evaluacionMensual = new EvaluacionMensual();
                    evaluacionMensual.ID = int.Parse(reader[0].ToString());
                    evaluacionMensual.IDCliente = int.Parse(reader[1].ToString());
                    evaluacionMensual.IDMes = int.Parse(reader[2].ToString());
                    evaluacionMensual.Calorias = int.Parse(reader[3].ToString());
                    evaluacionMensual.Altura = float.Parse(reader[4].ToString());
                    evaluacionMensual.Peso = float.Parse(reader[5].ToString());
                    evaluacionMensual.Grasa = float.Parse(reader[6].ToString());
                    evaluacionMensual.Comentarios = reader[7].ToString();

                    evaluacionMensuals.Add(evaluacionMensual);
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

            var count = evaluacionMensuals.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(EvaluacionMensual).Name.ToLower()} {from}-{to}/{count}");
            return evaluacionMensuals;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EvaluacionMensual>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM EvaluacionMensual WHERE ID = {id};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            EvaluacionMensual evaluacionMensual = new EvaluacionMensual();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    evaluacionMensual = new EvaluacionMensual();
                    evaluacionMensual.ID = int.Parse(reader[0].ToString());
                    evaluacionMensual.IDCliente = int.Parse(reader[1].ToString());
                    evaluacionMensual.IDMes = int.Parse(reader[2].ToString());
                    evaluacionMensual.Calorias = int.Parse(reader[3].ToString());
                    evaluacionMensual.Altura = float.Parse(reader[4].ToString());
                    evaluacionMensual.Peso = float.Parse(reader[5].ToString());
                    evaluacionMensual.Grasa = float.Parse(reader[6].ToString());
                    evaluacionMensual.Comentarios = reader[7].ToString();
                    connection.Close();
                    return evaluacionMensual;
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
        public async Task<ActionResult<EvaluacionMensual>> Post(EvaluacionMensual evaluacionMensual)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO EvaluacionMensual " +
                $"VALUES(" +
                $"{evaluacionMensual.IDCliente}, " +
                $"{evaluacionMensual.IDMes}, " +
                $"{evaluacionMensual.Calorias}, " +
                $"{evaluacionMensual.Altura}, " +
                $"{evaluacionMensual.Peso}, " +
                $"{evaluacionMensual.Grasa}, " +
                $"'{evaluacionMensual.Comentarios}'" +
                $");";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, evaluacionMensual);
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
        public async Task<ActionResult<EvaluacionMensual>> Put(int ID, EvaluacionMensual evaluacionMensual)
        {
            var entityId = (int)typeof(Cliente).GetProperty("ID").GetValue(evaluacionMensual);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE EvaluacionMensual SET " +
                $"IDCliente = {evaluacionMensual.IDCliente}, " +
                $"IDMes = {evaluacionMensual.IDMes}, " +
                $"Calorias = {evaluacionMensual.Calorias}, " +
                $"Altura = {evaluacionMensual.Altura}, " +
                $"Peso = {evaluacionMensual.Peso}, " +
                $"Grasa = {evaluacionMensual.Grasa}, " +
                $"Comentarios = '{evaluacionMensual.Comentarios}' " +
                $"WHERE ID = {evaluacionMensual.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, evaluacionMensual);
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
        public async Task<ActionResult<EvaluacionMensual>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM EvaluacionMensual WHERE ID = {ID};";
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
