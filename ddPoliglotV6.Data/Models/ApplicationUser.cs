using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ddPoliglotV6.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public bool IsSuperAdmin { get; set; }

        [NotMapped]
        public List<String> Roles { get; set; }

        public ShortUser MapToShortUser()
        {
            return new ShortUser { id = Id, name = UserName, roles = Roles.ToArray(), isLockedOut = (LockoutEnd ?? DateTime.Now) > DateTime.Now };
        }
    }

    public class ShortUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string[] roles { get; set; }
        public bool isLockedOut { get; set; }
    }
}
