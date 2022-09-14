using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Pages
{
    [BindProperties]
    public class ContactModel : PageModel
    {
        [Display(Name = "xxMessage"), Required(ErrorMessage = "xxMessage Required")]
        public string Message { get; set; }
        [Display(Name = "xxFirst Name"), Required(ErrorMessage = "xxFirst Name Required")]
        public string FirstName { get; set; }
        [Display(Name = "xxLast Name"), Required(ErrorMessage = "xxLast Name Required")]
        public string LastName { get; set; }
        [Display(Name = "xxEmail"), Required(ErrorMessage = "xxEmail Required"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}