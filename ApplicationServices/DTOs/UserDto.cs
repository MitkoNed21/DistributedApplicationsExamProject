using ApplicationServices.DTOs.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.DTOs
{
    public class UserDto : BaseDto, IValidatable
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string UserName { get; set; }
        
        public bool IsAdmin { get; set; }

        public override bool Validate()
        {
            return
                (String.IsNullOrWhiteSpace(FirstName) || FirstName.Length <= 40) &&
                (String.IsNullOrWhiteSpace(LastName) || LastName.Length <= 40) &&
                !String.IsNullOrWhiteSpace(UserName) && UserName.Length <= 40;/* &&
                !String.IsNullOrWhiteSpace(Token);*/
        }
    }
}
