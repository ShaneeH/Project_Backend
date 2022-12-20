namespace Backend_.Models
{


    public class UserData
    {
        public string Brand { get; set; }
        public int Clicks { get; set; }



        public UserData(string brand, int clicks)
        {
            Brand = brand;
            Clicks = clicks;
 
        }
    }


}
