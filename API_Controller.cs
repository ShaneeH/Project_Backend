using Backend_.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using Stripe;
using SendGrid;
using SendGrid.Helpers.Mail;
using Product = Backend_.Models.Product;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Backend_.Controllers
{
    [Route("[controller]")]
    [ApiController]


    public class DataController : ControllerBase
    {


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

   
            string query = "select * from mobile_direct.products";
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=";




            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Product> ProductList = new List<Product> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Console.WriteLine(reader["id"]);
                    Console.WriteLine(reader["name"]);
                    Console.WriteLine();
                    Console.WriteLine("--------- \n");


                    //CHANGES MADE HERE
                    Product p = new Product(
                        (int)reader["id"],
                        (string)reader["name"],
                        (float)reader["price"],
                        (string)reader["brand"],
                        (string)reader["desc"],
                        (string)reader["type"],
                        (string)reader["img"]
                        );
                    //CHANGES FINISHED

                    ProductList.Add(p);

                }
                conn.Close();
                return Ok(ProductList);
            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }


        }

        //Get Product Brands
        [EnableCors("AllowOrigin")]
        [HttpGet("~/Products/Brands")]
        public IActionResult getBrands()
        {


            List<String> Categories = new List<String> { };
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query = "SELECT DISTINCT(`brand`) FROM `mobile_direct`.`products`;";

            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);


            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Console.WriteLine(reader["brand"]);


                    Categories.Add(reader["brand"].ToString());

                }

                conn.Close();
                return Ok(Categories);


            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);

            }

        }

        //THIS Gets Data from the Anuglar APP and Then will use 
        //SendGrid to Send and Email to the Client
        [HttpPost("~/Mail")]
        public async Task<IActionResult> SendMail(JObject Payload)
        {
            //Get the Data from the Client JSON
            var name = Payload["name"];
            var amount = Payload["amount"];
            var items_amount = Payload["items_amount"];
            var email = Payload["email"];

            var apiKey = "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";

            //Create Email Object
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("shaneispro123@gmail.com", "Mobile Direct");
            var subject = $"Thanks {name} for the ORDER !!";
            var to = new EmailAddress(email.ToString(), "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";

            //This is the HTML the User will receive 
            String htmlContent = $"<head><style>body{{background-color: linen; text-align:center;}}h1{{color: maroon;}}</style></head><body><h1>MobileDirect</h1><br><br><h2> Thanks for the Order {name} </h2><h2> The order is on the way ! </h2><br><h3> You're Orer Total is {amount}euro for {items_amount} Items </h3><br><h5> MobileDirect would like to thank you for you're order </h5><br><h5> To view current orders and make changes please visit the Orders Section in the profile section on MobileDirect </h5><p> All orders are verified and processed by Stripe </p><p> For any problems be contact our support line at mobiledirect@mail.com</p></body>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return Ok("Email sent to " + name);
        }

      

        //This is for the Search feature on the FrontEnd
        [HttpPost("~/Search")]
        public async Task<IActionResult> Search(JObject Payload)


        {
            var search = Payload["search"];


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query1 = $"SELECT* FROM mobile_direct.products WHERE name LIKE '{search}%'";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Product> ProductList = new List<Product> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query1, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Console.WriteLine(reader["id"]);
                    Console.WriteLine(reader["name"]);
                    Console.WriteLine();
                    Console.WriteLine("--------- \n");



                    Product p = new Product(
                        (int)reader["id"],
                        (string)reader["name"],
                        (float)reader["price"],
                        (string)reader["brand"],
                        (string)reader["desc"],
                        (string)reader["type"],
                        (string)reader["img"]
                        );
        

                    ProductList.Add(p);

                }
                conn.Close();
                return Ok(ProductList);
            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

           
        }

        //Retrieve the Orders for each User by using there Email as the Identifier
        [HttpPost("~/Orders/Email")]
        public async Task<IActionResult> OrderByEmail(JObject Payload)


        {
            var email = Payload["email"];


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query1 = $"SELECT* FROM mobile_direct.orders WHERE email = '{email}'";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Order> OrderList = new List<Order> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query1, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    //CHANGES MADE HERE
                    Order o = new Order(
                        (int)reader["order_id"],
                        (string)reader["email"],
                        (DateTime)reader["date"],
                        (string)reader["products"],
                        (float)reader["total"]
                        );
                    //CHANGES FINISHED;

                    OrderList.Add(o);

                }
                conn.Close();
                return Ok(OrderList);
            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

           
        }


        //This is For Reccomending Products for the User 

        [HttpPost("~/Products/Reccomend")]
        public async Task<IActionResult> ProductsByReccomend(JObject Payload)


        {
            
            var brand = Payload["brand"];
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query = $"SELECT * FROM `mobile_direct`.`products`WHERE brand = '{brand}' AND type = 'ACCESSORY';";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Product> ProductList = new List<Product> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Product p = new Product(
                        (int)reader["id"],
                        (string)reader["name"],
                        (float)reader["price"],
                        (string)reader["brand"],
                        (string)reader["desc"],
                        (string)reader["type"],
                        (string)reader["img"]
                        );
                    //CHANGES FINISHED;

                    ProductList.Add(p);

                }
                conn.Close();
                return Ok(ProductList);
            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

           
        }


        // This Method is for Creating Orders

        [HttpPost("~/Orders/New")]
        public async Task<IActionResult> CreateOrder(JObject Payload)


        {
            var email = Payload["email"];
            var products = Payload["products"];
            var total = Payload["total"];



            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query = $"INSERT INTO `mobile_direct`.`orders`(`email`,`date`,`products`,`total`)VALUES('{email}',now(),'{products}', {total});";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);


            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();


            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

            return Ok("Order Created");
        }


        // This Method is for retriveing all Orders

        [HttpGet("~/Orders/All")]
        public async Task<IActionResult> GetOrders()
        {
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            
            string query = "SELECT `orders`.`order_id`,`orders`.`email`,`orders`.`date`,`orders`.`products`,`orders`.`total` FROM `mobile_direct`.`orders`";

            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);
            List<Order> OrderList = new List<Order> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
            
                    Order o = new Order(
                        (int)reader["order_id"],
                        (string)reader["email"],
                        (DateTime)reader["date"],
                        (string)reader["products"],
                        (float)reader["total"]
                        );
        

                    OrderList.Add(o);


                }
                return Ok(OrderList);
            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

           
        }


        //Get the total Amount of all the Orders
        [HttpGet("~/Orders/All/Amount")]
        public async Task<IActionResult> GetOrdersAmount()
        {

            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";
            string query = "SELECT SUM(Total) FROM mobile_direct.orders;";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();
                double amount = 0;

                while (reader.Read())
                {
                    amount = (double)reader["SUM(Total)"];           
                }

                  return Ok(amount);

            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

                 
        }


        //Get the Amount of Orders that have been placed
        [HttpGet("~/Orders/All/Quantity")]
        public async Task<IActionResult> GetOrdersQuantity()
        {

            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=";

            string query = "SELECT COUNT(order_id) FROM mobile_direct.orders;";

            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();
                Int64 qty = 0;

                while (reader.Read())
                {



                     qty = (Int64)reader["COUNT(order_id)"];


                }

                return Ok(qty);

            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }

            
        }

    }

}



