using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GymAPI.Config;
using GymAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase, IReactAdminController<Categoria>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Categoria";
            var t = new Categoria();

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

            List<Categoria> categorias = new List<Categoria>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                Categoria categoria;
                while (reader.Read())
                {
                    categoria = new Categoria();
                    categoria.ID = int.Parse(reader[0].ToString());
                    categoria.Nombre = reader[1].ToString();
                    categorias.Add(categoria);
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

            var count = categorias.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(Cliente).Name.ToLower()} {from}-{to}/{count}");

            return categorias;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> Get(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Categoria WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            Categoria categoria = new Categoria();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    categoria.ID = int.Parse(reader[0].ToString());
                    categoria.Nombre = reader[1].ToString();
                    connection.Close();
                    Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
                    return categoria;
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
        public async Task<ActionResult<Categoria>> Post(Categoria categoria)
        {

            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO CATEGORIA VALUES('{categoria.Nombre}');";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, categoria);
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
        public async Task<IActionResult> Put(int ID, Categoria categoria)
        {
        
            var entityId = (int)typeof(Cliente).GetProperty("ID").GetValue(categoria);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE CATEGORIA SET Nombre = '{categoria.Nombre}' WHERE ID = {categoria.ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, categoria);
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
        public async Task<ActionResult<Categoria>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM Categoria WHERE ID = {ID};";
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
