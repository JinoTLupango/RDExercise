using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RDExerciseAPI.Model;
using System.Data;

namespace RDExerciseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost]
        public IActionResult SaveData([FromBody] Register model)
        {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("sp_RDExercise", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Firstname", model.Firstname);
                cmd.Parameters.AddWithValue("@Middlename", model.Middlename);
                cmd.Parameters.AddWithValue("@Lastname", model.Lastname);
                cmd.Parameters.AddWithValue("@Address", model.Address);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Username", model.Username);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                // return value
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParameter.Value;

                    if (result == 200)
                        return Ok(new { Message = "User registered successfully!" });

                    return StatusCode(500, new { Message = "Insert failed." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = ex.Message });
                }
            }
        }


        [HttpGet(Name = "get")]
        public IActionResult Get()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<Register> list = new List<Register>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Firstname, Middlename, Lastname, Address, Email, Username, [Password] FROM RDExercise";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Register
                    {
                        Firstname = reader["Firstname"].ToString(),
                        Middlename = reader["Middlename"].ToString(),
                        Lastname = reader["Lastname"].ToString(),
                        Address = reader["Address"].ToString(),
                        Email = reader["Email"].ToString(),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString()
                    });
                }
                conn.Close();
            }

            return Ok(list);
        }


    }
}
