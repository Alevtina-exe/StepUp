using StepUp.Domain.Enums;

namespace StepUp.Domain.Entities
{
    public class User
    {
        private User () { }
        public User (string fullName, string email, string password,
            DateTime birthDate, Gender gender, double weight, double height, 
            int caloriePlan, int carbonPercent, int proteinPercant, int fatPercent,
            SportLevel sportLevel)
        {
            Id = Guid.NewGuid ();
            FullName = fullName;
            Email = email;
            BirthDate = birthDate;
            Gender = gender;
            Weight = weight;
            Height = height;
            CaloriePlan = caloriePlan;
            CarbonPercent = carbonPercent;
            ProteinPercent = proteinPercant;
            FatPercent = fatPercent;
            SportLevel = sportLevel;
            RegistrationDate = DateTime.Now;
            Password = password;
        }
        public Guid Id { get; private set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; 
        public DateTime BirthDate { get; set; }
        public Gender Gender{ get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public int CaloriePlan { get; set; }
        public int CarbonPercent { get; set; }
        public int ProteinPercent { get; set; }
        public int FatPercent { get; set; }
        public SportLevel SportLevel { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int Age => DateTime.Today.Year - BirthDate.Year;
    }
}
