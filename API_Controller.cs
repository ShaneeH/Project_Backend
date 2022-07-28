using Backend_.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend_.Controllers
{
    [Route("[controller]")]
    [ApiController]

 
    public class DataController : ControllerBase{


        [EnableCors("AllowOrigin")]
        [HttpGet("~/Users/All")]
        public IActionResult Get()
        {

            string select = "select * from ang_test.people";
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=redking";

            string select2 = "select * from sys.users_test";
            string server2 = "server=project.ccdf0fmr7cmw.eu-west-1.rds.amazonaws.com;port=3306;database=sys;uid=admin;password=redking97";
          


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<User> UserList = new List<User> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(select, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Console.WriteLine(reader["id"]);
                    Console.WriteLine(reader["email"]);
                    Console.WriteLine();
                    Console.WriteLine("--------- \n");

                    //Create JSON Obj to send to Client
                    //var User_Json = new{ 
                      //            id = reader["id"],
                        //          Email = reader["email"],
                          //        Msg = reader["msg"]
                            //    };


                    //string? v = reader["id"].ToString();
                    //String UserID = v;

                    User Person = new User((int)reader["id"], (string)reader["email"], (string)reader["msg"]);
                    UserList.Add(Person);

                    //List.Add(User_Json);
                }

                return Ok(UserList);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

            return Ok("Hi");
        }

        
        [HttpPost("~/Users/Create")]
        //[ValidateAntiForgeryToken]
        public IActionResult Post(JObject SQL_CMD)
        {
            Console.WriteLine("POST Method has been HIT !!!!");
            String sql = SQL_CMD.ToString();
            dynamic data = JObject.Parse(sql);
            Console.WriteLine(data.CMD);

            //Declare Variables with SQL Commands
            string insert = "INSERT INTO ang_test.people (id,email,msg) VALUES (9,'BillyYayo@wizzmail.com', 'Hey Everyones');";
            string select = "select * from ang_test.people";
            string conn1 = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=redking";

            String Sql_string = data.CMD.ToString();

            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(conn1);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Sql_string, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();

            }
            catch (SqlException ex)
            {
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }
            return Ok(data.CMD);

        }



        [HttpGet("~/Test")]
        public IActionResult Test()
        {

            return Ok("Hi im the API and im Active (:");
        }

        //ORDERS SECTION OF THE API
        [HttpPost("~/Orders/Create")]
        public IActionResult POST(JObject SQL_CMD)
        {

            return Ok("Hi im the POST Orders API " + SQL_CMD); ;
        }


        //Get All Products
        [EnableCors("AllowOrigin")]
        [HttpGet("~/Products/All")]
        public IActionResult GetProducts()
        {

            string select = "select * from ang_test.products";
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=redking";

            string select2 = "select * from sys.users_test";
            string server2 = "server=project.ccdf0fmr7cmw.eu-west-1.rds.amazonaws.com;port=3306;database=sys;uid=admin;password=redking97";



            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Product> ProductList = new List<Product> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(select, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Console.WriteLine(reader["id"]);
                    Console.WriteLine(reader["name"]);
                    Console.WriteLine();
                    Console.WriteLine("--------- \n");

      

                    Product p = new Product((int)reader["id"], (string)reader["name"],  (int)reader["price"], 
                        (string)reader["img"], (string)reader["category"]);
                    ProductList.Add(p);

                }

                return Ok(ProductList);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

            return Ok("Hi");
        }


    }
}
