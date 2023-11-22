using NetworkApp.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.Data
{
    public class GenetateUsers
    {
        public List<User> Populate(int count)
        {
            var users = new List<User>();
            for (int i = 1; i < count; i++)
            {
                var rand = new Random();

                string lastName = rand.Next(4, 10).ToString();
                string firstName = rand.Next(11, 20).ToString();
                string pictureNumber = rand.Next(1, 5).ToString();

                var item = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.Now.AddDays(-rand.Next(1, (DateTime.Now - DateTime.Now.AddYears(-25)).Days)),
                    Email = firstName + lastName + "@test.com",
                };

                item.UserName = item.Email;
                item.Image = "/Static/" + pictureNumber + ".png";

                users.Add(item);
            }

            return users;
        }
    }
}
