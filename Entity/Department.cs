namespace Entity
{
    public class Department
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Company Company { get; set; }
    }
}
