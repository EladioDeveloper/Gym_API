using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Gym_API.Config;
using Gym_API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gym_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // GET: api/<AdminController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAll()
        {
            Connection conex = new Connection();
            SqlConnection connection = new SqlConnection(conex.connectionString);
            string sql = "SELECT ID, Usuario FROM Administrador;";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader;
            connection.Open();

            List<Admin> adminList = new List<Admin>();
            try
            {
                reader = cmd.ExecuteReader();
                Admin admin;
                while(reader.Read())
                {
                    admin = new Admin();
                    admin.ID = int.Parse(reader[0].ToString());
                    admin.Usuario = reader[1].ToString();
                    adminList.Add(admin);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return adminList;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AdminController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AdminController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
