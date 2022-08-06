namespace Backend_.Models
{


    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Brand { get; set; }
        public string Desc { get; set; }
        public string Type { get; set; }
        public string Img { get; set; }

        




        public Product(int id, string name, float price, string brand, string desc, string type, string img)
        {
            Id = id;
            Name = name;
            Price = price;
            Brand = brand;  
            Desc = desc;    
            Type = type;
            Img = img;
        }
    }


}
