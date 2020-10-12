using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
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
    public class ClienteController : ControllerBase, IReactAdminController<Cliente>
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> Get(string filter = "", string range = "", string sort = "")
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Cliente";
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

            List<Cliente> clientes = new List<Cliente>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                Cliente cliente;
                while (reader.Read())
                {
                    cliente = new Cliente();
                    cliente.ID = int.Parse(reader[0].ToString());
                    cliente.IDRutina = int.Parse(reader[1].ToString());
                    cliente.IDDireccion = int.Parse(reader[2].ToString());
                    cliente.IDInscripcion = int.Parse(reader[3].ToString());
                    cliente.Nombre = reader[4].ToString();
                    cliente.Apellido = reader[5].ToString();
                    cliente.Telefono = reader[6].ToString();
                    cliente.Email = reader[7].ToString();
                    cliente.FNacimiento = Convert.ToDateTime(reader[8].ToString());
                    cliente.FRegistro = Convert.ToDateTime(reader[9].ToString());

                    clientes.Add(cliente);
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

            var count = clientes.Count();

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Range");
            Response.Headers.Add("Content-Range", $"{typeof(Cliente).Name.ToLower()} {from}-{to}/{count}");
            return clientes;


        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> Get(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Cliente WHERE ID = {ID};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            connection.Open();
            Cliente cliente = new Cliente();

            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    cliente.ID = int.Parse(reader[0].ToString());
                    cliente.IDRutina = int.Parse(reader[1].ToString());
                    cliente.IDDireccion = int.Parse(reader[2].ToString());
                    cliente.IDInscripcion = int.Parse(reader[3].ToString());
                    cliente.Nombre = reader[4].ToString();
                    cliente.Apellido = reader[5].ToString();
                    cliente.Telefono = reader[6].ToString();
                    cliente.Email = reader[7].ToString();
                    cliente.FNacimiento = Convert.ToDateTime(reader[8].ToString());
                    cliente.FRegistro = Convert.ToDateTime(reader[9].ToString());
                    connection.Close();
                    return cliente;
                } else {
                    connection.Close();
                    return NotFound();
                }
            }
            catch(Exception ex) {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
            
            finally
            {
                connection.Close();
            }

        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> Post(Cliente cliente)
        {
            cliente.FRegistro = DateTime.Now;
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO CLIENTE VALUES(" +
                $"{cliente.IDRutina}, " +
                $"{cliente.IDDireccion}, " +
                $"{cliente.IDInscripcion}, " +
                $"'{cliente.Nombre}', " +
                $"'{cliente.Apellido}', " +
                $"'{cliente.Telefono}', " +
                $"'{cliente.Email}', " +
                $"'{cliente.FNacimiento}', " +
                $"'{cliente.FRegistro}');";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, cliente);
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
        public async Task<IActionResult> Put(int ID, Cliente cliente)
        {
            var entityId = (int)typeof(Cliente).GetProperty("ID").GetValue(cliente);
            if (ID != entityId)
            {
                return BadRequest();
            }
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE CLIENTE SET " +
                $"IDRutina = {cliente.IDRutina}, " +
                $"IDDireccion = {cliente.IDDireccion}, " +
                $"IDInscripcion = {cliente.IDInscripcion}, " +
                $"Nombre = '{cliente.Nombre}', " +
                $"Apellido = '{cliente.Apellido}', " +
                $"Telefono = '{cliente.Telefono}', " +
                $"Email = '{cliente.Email}', " +
                $"FNacimiento = '{cliente.FNacimiento}', " +
                $"FRegistro = '{cliente.FRegistro}' " +
                $"WHERE ID = {cliente.ID};";

            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, cliente);
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
        public async Task<ActionResult<Cliente>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM Cliente WHERE ID = {ID};";
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
