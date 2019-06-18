using System;

namespace ParametersExtractor.ConsoleApp.Models
{
    public sealed class User
    {
        public string Name { get; set; } = "Виктор";

        public int Age { get; set; } = 21;

        public DateTime BirthDate { get; set; } = new DateTime(1998, 05, 12);

        public bool Active { get; set; }

        public UserRole Role { get; set; } = new UserRole();

        public User(bool active)
        {
            Active = active;
        }
    }
}