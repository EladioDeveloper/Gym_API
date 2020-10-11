using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GymAPI.Config;
using GymAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT ID, Nombre FROM Categorias;";
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

            return categorias;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Categoria WHERE ID = {id};";
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
                    connection.Close();
                    categoria.ID = int.Parse(reader[0].ToString());
                    categoria.Nombre = reader[1].ToString();
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
                    return StatusCode(200, "Se inserto correctamente");
                else
                    return StatusCode(501, "No se pudo registrar");
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<Admin>> Put(Categoria categoria)
        {
        
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE CATEGORIA SET Nombre = '{categoria.Nombre}';";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            connection.Open();
            try
            {
                int x = await cmd.ExecuteNonQueryAsync();
                if (x > 0)
                    return StatusCode(200, "Se modifico correctamente");
                else
                    return StatusCode(501, "No se pudo modificar");
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
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
