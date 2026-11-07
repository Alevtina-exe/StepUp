using StepUp.Domain.Enums;

namespace StepUp.Domain.Entities
{
    public class User
    {
        private User() { }
        public User(string fullName, string email, string password,
            DateTime birthDate, Gender gender, double weight, double height,
            int caloriePlan, int carbonPercent, int proteinPercent, int fatPercent,
            SportLevel sportLevel, DateTime registrationDate)
        {
            Id = Guid.NewGuid();
            Email = email;
            ChangeFullName(fullName);
            ChangeWeight(weight);
            ChangeHeight(height);
            ChangeCaloriePlan(caloriePlan);
            ChangeMacros(proteinPercent, fatPercent, carbonPercent);
            ChangeBirthDate(birthDate);
            Gender = gender;
            SportLevel = sportLevel;
            RegistrationDate = registrationDate;
            ChangePassword(password);
        }
        public Guid Id { get; }
        public string Email { get; } = null!;
        public string Password { get; private set; } = null!;
        public DateTime RegistrationDate { get; }

        public string FullName { get; private set; } = null!;
        public DateTime BirthDate { get; private set; }
        public Gender Gender { get; private set; }
        public double Weight { get; private set; }
        public double Height { get; private set; }
        public int CaloriePlan { get; private set; }
        public int CarbonPercent { get; private set; }
        public int ProteinPercent { get; private set; }
        public int FatPercent { get; private set; }
        public SportLevel SportLevel { get; private set; }

        public int Age
        {
            get
            {
                var today = DateTime.UtcNow.Date; 
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }


        public void ChangeFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Имя не может быть пустым.", nameof(fullName));
            FullName = fullName.Trim();
        }

        public void ChangeWeight(double weight)
        {
            if (weight < 30 || weight > 300)
                throw new ArgumentOutOfRangeException(nameof(weight), "Вес должен быть в диапазоне 30-300 кг.");
            Weight = weight;
        }

        public void ChangeHeight(double height)
        {
            if (height < 100 || height > 250)
                throw new ArgumentOutOfRangeException(nameof(height), "Рост должен быть в диапазоне 100-250 см.");
            Height = height;
        }

        public void ChangeCaloriePlan(int caloriePlan)
        {
            if (caloriePlan < 1300)
                throw new ArgumentOutOfRangeException(nameof(caloriePlan), "Дневная норма калорий не может быть меньше 1300.");
            CaloriePlan = caloriePlan;
        }

        public void ChangeMacros(int proteinPercent, int fatPercent, int carbonPercent)
        {
            if (proteinPercent < 1 || proteinPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(proteinPercent), "Белки: 1-100%");
            if (fatPercent < 1 || fatPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(fatPercent), "Жиры: 1-100%");
            if (carbonPercent < 1 || carbonPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(carbonPercent), "Углеводы: 1-100%");

            if (proteinPercent + fatPercent + carbonPercent != 100)
                throw new ArgumentException("Сумма БЖУ должна быть ровно 100%.");

            ProteinPercent = proteinPercent;
            FatPercent = fatPercent;
            CarbonPercent = carbonPercent;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Пароль не может быть пустым.", nameof(newPasswordHash));
            Password = newPasswordHash;
        }

        public void ChangeGender(Gender gender) => Gender = gender;

        public void ChangeSportLevel(SportLevel sportLevel) => SportLevel = sportLevel;

        public void ChangeBirthDate(DateTime birthDate)
        {
            if (birthDate.Date > DateTime.Today)
                throw new ArgumentException("Дата рождения не может быть в будущем.", nameof(birthDate));
            BirthDate = birthDate.Date;
        }
    }
}

