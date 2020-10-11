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
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GymAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inscripcion>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Inscripcion;";
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

            return inscripcions;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscripcion>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM Inscripcion WHERE ID = {id};";
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
                    connection.Close();
                    inscripcion = new Inscripcion();
                    inscripcion.ID = int.Parse(reader[0].ToString());
                    inscripcion.IDPlan = int.Parse(reader[1].ToString());
                    inscripcion.FPago = Convert.ToDateTime(reader[2].ToString());
                    inscripcion.FExpiracion = Convert.ToDateTime(reader[3].ToString());
                    inscripcion.AutoRenovacion = Convert.ToBoolean(reader[4].ToString());
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
                $"{inscripcion.AutoRenovacion}, " +
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
        public async Task<ActionResult<Inscripcion>> Put(Inscripcion inscripcion)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"UPDATE Inscripcion SET " +
                $"IDPlan = {inscripcion.IDPlan}, " +
                $"FPago = '{inscripcion.FPago}', " +
                $"FExpiracion = '{inscripcion.FExpiracion}', " +
                $"AutoRenovacion = {inscripcion.AutoRenovacion}, " +
                $"WHERE ID = {inscripcion.ID};";
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