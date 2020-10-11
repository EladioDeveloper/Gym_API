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
    public class EjercicioRutinaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EjercicioRutina>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM EjercicioRutina;";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            connection.Open();

            List<EjercicioRutina> ejercicioRutinas = new List<EjercicioRutina>();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                EjercicioRutina ejercicioRutina;
                while (reader.Read())
                {
                    ejercicioRutina = new EjercicioRutina();
                    ejercicioRutina.ID = int.Parse(reader[0].ToString());
                    ejercicioRutina.IDCategoria = int.Parse(reader[1].ToString());
                    ejercicioRutina.Nombre = reader[2].ToString();
                    ejercicioRutinas.Add(ejercicioRutina);
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

            return ejercicioRutinas;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EjercicioRutina>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM EjercicioRutina WHERE ID = {id};";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;

            EjercicioRutina ejercicioRutina = new EjercicioRutina();
            connection.Open();
            try
            {
                reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    connection.Close();
                    ejercicioRutina.ID = int.Parse(reader[0].ToString());
                    ejercicioRutina.IDCategoria = int.Parse(reader[1].ToString());
                    ejercicioRutina.Nombre = reader[2].ToString();
                    return ejercicioRutina;
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
        public async Task<ActionResult<EjercicioRutina>> Post(EjercicioRutina ejercicioRutina)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"INSERT INTO EjerciciRutina " +
                $"VALUES({ejercicioRutina.IDCategoria}," +
                $"'{ejercicioRutina.Nombre}');";
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
        public async Task<ActionResult<EjercicioRutina>> Put(EjercicioRutina ejercicioRutina)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE EjercicioRutina SET " +
                $"IDCategoria = {ejercicioRutina.IDCategoria}, " +
                $"Nombre = '{ejercicioRutina.Nombre}' " +
                $"WHERE ID = {ejercicioRutina.ID};";
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
        public async Task<ActionResult<EjercicioRutina>> Delete(int ID)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"DELETE FROM EjercicioRutina WHERE ID = {ID};";
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