namespace Entity
{
    public class Passport
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }

        public Passport(int id, string type, string number)
        {
            Id = id;
            Type = type;
            Number = number;
        }
    }
}
