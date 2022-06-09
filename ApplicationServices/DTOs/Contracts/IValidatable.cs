using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.DTOs.Contracts
{
    public interface IValidatable
    {
        public bool Validate();
    }
}
