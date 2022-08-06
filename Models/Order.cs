namespace Backend_.Models
{
    public class Order
    {
        public int Order_id { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string Products { get; set; }
        public float Total { get; set; }



        public Order(int order_id, string email, DateTime date, string products, float total)
        {
            Order_id = order_id;
            Email = email;
            Date = date;
            Products = products;
            Total = total;

        }
    }
}
