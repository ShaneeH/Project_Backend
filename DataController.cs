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
using UserData = Backend_.Models.UserData;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using RestSharp;

namespace Backend_.Controllers
{
    [Route("[controller]")]
    [ApiController]


    public class DataController : ControllerBase
    {

        /*
         * author @ShaneHahesy
         * This is an API used to Interact with the Angular Front-End and The SQL DataBase 
         * The Front-End will post Data to this Application witch will then be used for further Computation
         * And in return sent back to the Front-End 
         * The Back-End Talks Directly the SQL Server which hosts all the Data for the Front-End
         * The Front-End and SQL Server never talk directly to eachother
         
    
         *  Front-End ->   Back-End   ->  SQL Server
         *            <-    *****      <- Server
         *    
           
         * For this Back-End to work Locally we need to disable the CORS Policy, I created a document on how to do this 
         * see here https://docs.google.com/document/d/e/2PACX-1vSODf7QJLjFVKFu4TkXxOJ8BxDWfxQrHA4wTvBOvQEeyVumf0zI2P8bweFNpgtBNcKRF8_8xQDYXO89/pub
         */

        //This Method is just to test to see if the API is Online 
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
        //This is to Display all Products in the Shop
        [EnableCors("AllowOrigin")]
        [HttpGet("~/Products/All")]
        public IActionResult GetProducts()
        {
            string select3 = "select * from mobile_direct.products";
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=password";
            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);
            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<Product> ProductList = new List<Product> { };
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(select3, conn);
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


        //If we need to get a Certain Product based on it's ID this Method is Called
        [HttpPost("~/Products/id")]
        public async Task<IActionResult> GetProductID(JObject Payload)


        {
            var id = Payload["id"];


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
            string query1 = $"SELECT* FROM mobile_direct.products WHERE id = '{id}%'";


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

        //Get Product Brands this will return all the Products Brand
        //This is used for Filtering in the Store
        [EnableCors("AllowOrigin")]
        [HttpGet("~/Products/Brands")]
        public IActionResult getBrands()
        {


            List<String> Categories = new List<String> { };
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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
            var email_string = email.ToString();
            var sendgrid_apikey = "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";

            //Create Email Object
            var client = new SendGridClient(sendgrid_apikey);
            var from = new EmailAddress("shaneispro123@gmail.com", "Mobile Direct");
            var subject = $"Thanks {name} for the ORDER !!";
            if (email_string == "")
            {
                email_string = "Guest User";
            }

            var to = new EmailAddress(email_string, "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";

            //This is the HTML the User will receive 
            String htmlContent = $"<head><style>body{{background-color: linen; text-align:center;}}h1{{color: maroon;}}</style></head><body><h1>MobileDirect</h1><br><br><h2> Thanks for the Order {name} </h2><h2> The order is on the way ! </h2><br><h3> You're Orer Total is {amount}euro for {items_amount} Items </h3><br><h5> MobileDirect would like to thank you for you're order </h5><br><h5> To view current orders and make changes please visit the Orders Section in the profile section on MobileDirect </h5><p> All orders are verified and processed by Stripe </p><p> For any problems be contact our support line at mobiledirect@mail.com</p></body>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return Ok("Email sent to " + name);
        }

        [HttpPost("~/EMail")]
        public async Task<IActionResult> SendEMail(JObject Payload)
        {



            return Ok();

        }



        //This is for the Search feature on the FrontEnd
        [HttpPost("~/Search")]
        public async Task<IActionResult> Search(JObject Payload)


        {
            var search = Payload["search"];


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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

                    Order o = new Order(
                        (int)reader["order_id"],
                        (string)reader["email"],
                        (DateTime)reader["date"],
                        (string)reader["products"],
                        (float)reader["total"]
                        );


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
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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


        // This Method is for Creating Orders and Inserting them into the DataBase

        [HttpPost("~/Orders/New")]
        public async Task<IActionResult> CreateOrder(JObject Payload)


        {
            var email = Payload["email"];
            var products = Payload["products"];
            var total = Payload["total"];



            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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
            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
            string query = "Select * from mobile_direct.orders";
            string q = "SELECT `orders`.`order_id`,`orders`.`email`,`orders`.`date`,`orders`.`products`,`orders`.`total` FROM `mobile_direct`.`orders`";

            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);
            List<Order> OrderList = new List<Order> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(q, conn);
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

            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";
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

            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";

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


        //Admins can Create Products from the Adminstrator Dashboard in the Front-End
        //This inserts the new Product in the DataBase
        [HttpPost("~/Products/Create")]
        public async Task<IActionResult> CreateProduct(JObject Payload)

        {

            var name = Payload["name"];
            var price = Payload["price"];
            var brand = Payload["brand"];
            var desc = Payload["desc"];
            var type = Payload["type"];
            var img = Payload["img"];


            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";

            string query = $"INSERT INTO `mobile_direct`.`products`(`name`,`price`,`brand`,`desc`,`type`,`img`)VALUES('{name}',{price}, '{brand}','{desc}','{type}','{img}')";




            MySqlConnection conn = new MySqlConnection(server);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();



                return Ok("product added");

            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }


            return null;


        }

        //Admins can Delete Products from the Adminstrator Dashboard in the Front-End
        //This Deletes  new Product in the DataBase
        [HttpPost("~/Product/Delete")]
        public async Task<IActionResult> DeleteProduct(JObject Payload)

        {

            var id = (int)Payload["id"];



            string server = "server=127.0.0.1;port=3306;database=mobile_direct;uid=root;password=password";

            string query = $"DELETE FROM mobile_direct.products where id = {id}";




            MySqlConnection conn = new MySqlConnection(server);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();



                return Ok("product deleted");

            }
            catch (SqlException ex)
            {
                conn.Close();
                Console.WriteLine("there was an issue!", ex);
                return Ok(ex);
            }


            return null;


        }


        //DATA TRACKING METHODS :

        /*
         * The Next Part of the API is the Data Tracking Section
         * When a User Clicks on view Item in the Front-End will we store these Clicks in the DataBase
         * Each User will given there Own Table with there Data Stored in this Table 
         * When a User Clicks on a Product for example 'iPhone 13' we will create a new column called 'Apple' and everytime the user clicks an Apple Product increase the APPLE Value
         * This is so we can learn about the User's Shopping Habbits and Interests 
         * 
         */


        //This will create a DB For the User with there UserName as the Table Name

        [HttpPost("~/User/CreateDB")]
        public async Task<IActionResult> TestDB(JObject Payload)

        {



            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=password";

            string brand = (string)Payload["brand"];
            string email = (string)Payload["email"];




            string query1 = $"CREATE TABLE `ang_test`.`{email}` (`Brand` VARCHAR(255) NOT NULL,`Clicks` INT NULL,PRIMARY KEY(`Brand`))";


            MySqlConnection conn = new MySqlConnection(server);

            //QUERY One: Create new Table for User
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query1, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();
                conn.Close();

            }
            catch (SqlException ex)
            {


            }



            return Ok("All Done");


        }

        //This will insert the new Brand into the DataBase if 

        [HttpPost("~/User/CreateBrand")]
        public async Task<IActionResult> TestDB2(JObject Payload)

        {


            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=password";

            string brand = (string)Payload["brand"];
            string email = (string)Payload["email"];



            //This will Insert a new brand into the Table 
            string query2 = $"INSERT INTO `ang_test`.`{email}`(`Brand`,`Clicks`) VALUES ('{brand}' , 0)";



            MySqlConnection conn = new MySqlConnection(server);

            //QUERY One: Create new Table for User
            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query2, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();
                conn.Close();

            }
            catch (SqlException ex)
            {


            }

            return Ok("All Done");



        }



        //THIRD METHOD OF DATA TRACKING

        [HttpPost("~/User/UpdateBrand")]
        public async Task<IActionResult> TestDB3(JObject Payload)

        {


            //This will increment the Brand by One Click
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=password";

            string brand = (string)Payload["brand"];
            string email = (string)Payload["email"];



            string query3 = $"UPDATE {email} SET Clicks = Clicks + 1 WHERE Brand = '{brand}'  ";



            MySqlConnection conn = new MySqlConnection(server);



            try
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query3, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();
                conn.Close();





            }
            catch (SqlException ex)
            {


            }

            return Ok("All Done");

        }



        //If we need to get a Certain Product based on it's ID this Method is Called
        [HttpPost("~/User/Data")]
        public async Task<IActionResult> GetProductData(JObject Payload)


        {
            string email = (string)Payload["email"];

            //Get the User Data and Order there Most Clicked Brands by Descending

            string select3 = $"select * from ang_test.{email} ORDER BY Clicks DESC";
            string server = "server=127.0.0.1;port=3306;database=ang_test;uid=root;password=password";


            //Open the SQL Connection
            MySqlConnection conn = new MySqlConnection(server);

            //This ArrayList will hold the User Objects as defined in the User.cs class
            List<UserData> DataList = new List<UserData> { };

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(select3, conn);
                Console.WriteLine("SQL Command Executed");
                MySqlDataReader reader = cmd.ExecuteReader();



                while (reader.Read())
                {
                    Console.WriteLine(reader["Brand"]);
                    Console.WriteLine(reader["Clicks"]);
                    Console.WriteLine();
                    Console.WriteLine("--------- \n");

                    UserData d = new UserData((string)reader["Brand"], (int)reader["Clicks"]);

                    DataList.Add(d);

                }

                conn.Close();

                return Ok(DataList);
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
