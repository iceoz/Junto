using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Entity
{
    public class User
    {
        public User()
        {

        }

        public User(string login, string name, string password)
        {
            Name = name;
            Login = login;
            Password = password;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
