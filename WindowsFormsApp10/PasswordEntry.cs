using System;
namespace PasswordManager // Замените PasswordManager на имя вашего проекта
{
    public class PasswordEntry
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public PasswordEntry(string login, string password, string
       description)
        {
            Login = login;
            Password = password;
            Description = description;
        }
        public override string ToString()
        {
            return $"{Login} - {Description}"; // Отображение в ListBox
        }
    }
}

