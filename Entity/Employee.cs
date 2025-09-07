namespace Entity
{
    public class Employee
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int PassportId { get; set; }
        public int DeparmentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public Passport Passport { get; set; }
        public Department Department { get; set; }

        public Employee(int id, 
            int companyId, 
            int passportId,
            int departmentId,
            string name, 
            string surname, 
            string phone)
        {
            Id = id;
            CompanyId = companyId;
            Name = name;
            Surname = surname;
            Phone = phone;
        }
    }
}
