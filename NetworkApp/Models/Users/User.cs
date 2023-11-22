using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.Models.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public string About { get; set; }

        public User()
        {
            Image = "Static/500.png"; //Не будем генерировать лишний трафик, качая картинки
            Status = "Зарегистрирован";
            About = "Добавьте сюда информацию о себе";
        }

        public string GetFullName()
        {
            return FirstName + " " + MiddleName + " " + LastName;
        }
    }
}
