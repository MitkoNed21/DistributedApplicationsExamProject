using ApplicationServices.DTOs.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.DTOs
{
    public abstract class BaseDto : IValidatable
    {
        public int Id { get; set; }

        public abstract bool Validate();
    }
}
