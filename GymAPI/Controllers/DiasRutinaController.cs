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
    public class DiasRutinaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiasRutina>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM DiasRutina;";
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
                    diasRutina.IDDia = int.Parse(reader[1].ToString());
                    diasRutina.IDEjercicio = int.Parse(reader[2].ToString());
                    diasRutina.Repeticiones = int.Parse(reader[3].ToString());
                    diasRutina.Series = int.Parse(reader[4].ToString());
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

            return diasRutinas;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiasRutina>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM DiasRutina WHERE ID = {id};";
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
                    connection.Close();
                    diaRutina.ID = int.Parse(reader[0].ToString());
                    diaRutina.IDDia = int.Parse(reader[1].ToString());
                    diaRutina.IDEjercicio = int.Parse(reader[2].ToString());
                    diaRutina.Repeticiones = int.Parse(reader[3].ToString());
                    diaRutina.Series = int.Parse(reader[4].ToString());
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
            string sql = $"INSERT INTO DiasRutina " +
                $"VALUES(" +
                $"IDRutina = {diasRutina.IDRutina}, " +
                $"IDDia = {diasRutina.IDDia}, " +
                $"IDEjercicio = {diasRutina.IDEjercicio}, " +
                $"Repeticiones = {diasRutina.Repeticiones}, " +
                $"Series = {diasRutina.Series}" +
                $");";
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
        public async Task<ActionResult<DiasRutina>> Put(DiasRutina diasRutina)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE DiasRutina SET " +
                $"IDRutina = {diasRutina.IDRutina}, " +
                $"IDDia = {diasRutina.IDDia}, " +
                $"IDEjercicio = {diasRutina.IDEjercicio}, " +
                $"Repeticiones = {diasRutina.Repeticiones}, " +
                $"Series = {diasRutina.Series}, " +
                $"WHERE ID = {diasRutina.ID};";
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
