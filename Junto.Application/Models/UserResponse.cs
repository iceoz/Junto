using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Application.Models
{
    public class UserResponse
    {
        public UserResponse()
        { }

        public UserResponse(int id, string name, string login)
        {
            Id = id;
            Name = name;
            Login = login;
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
    }
}
