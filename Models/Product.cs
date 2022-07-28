namespace Backend_.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Img { get; set; }
        public string Category { get; set; }




        public Product(int id, string name, int price, string img, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Img = img;
            Category = category;
        }
    }
}
