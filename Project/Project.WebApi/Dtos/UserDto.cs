using System.Collections.Generic;

namespace Project.WebAPI.Dtos
{
    public class UserDto
    {
        public string id {get;set;}
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
}