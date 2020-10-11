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
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> Get()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT * FROM Cliente;";
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

            return clientes;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetID(int ID)
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
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return cliente;

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
        public async Task<ActionResult<Cliente>> Put(Cliente cliente)
        {

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
                $"FRegistro = '{cliente.FRegistro}', " +
                $"WHERE ID = {cliente.ID};";
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
