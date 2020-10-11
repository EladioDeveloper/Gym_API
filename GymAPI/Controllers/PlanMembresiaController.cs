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
    public class PlanMembresiaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanMembresia>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM PlanMembresia;";
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

            return planMembresias;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanMembresia>> Get(int id)
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = $"SELECT * FROM PlanMembresia WHERE ID = {id};";
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
                    connection.Close();
                    planMembresia.ID = int.Parse(reader[0].ToString());
                    planMembresia.Nombre = reader[1].ToString();
                    planMembresia.Descripcion = reader[2].ToString();
                    planMembresia.TiempoValidez = int.Parse(reader[3].ToString());
                    planMembresia.Monto = float.Parse(reader[4].ToString());
                    planMembresia.Estado = Convert.ToBoolean(reader[5].ToString());
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
                $"{planMembresia.Estado}" +
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
        public async Task<ActionResult<PlanMembresia>> Put(PlanMembresia planMembresia)
        {

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
