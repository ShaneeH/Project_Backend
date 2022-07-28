namespace Backend_.Models
{
    public class User
 {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }


        public User(int id, string name, string msg){
            Id = id;
            Name = name;
            Msg = msg;              
        }
    }
}
