using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Project.Domain.Identity
{
    public class User : IdentityUser<int>
    {
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public bool isAdmin { get; set; }
        public Employee Employees { get; set; }
    }
}