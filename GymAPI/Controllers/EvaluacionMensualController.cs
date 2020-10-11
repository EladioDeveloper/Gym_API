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
    public class EvaluacionMensualController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvaluacionMensual>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM EvaluacionMensual;";
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
        public async Task<ActionResult<EvaluacionMensual>> Put(EvaluacionMensual evaluacionMensual)
        {
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
