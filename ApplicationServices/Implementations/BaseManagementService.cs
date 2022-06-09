using ApplicationServices.DTOs;
using ApplicationServices.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Implementations
{
    public abstract class BaseManagementService<TEntityDto> where TEntityDto : BaseDto
    {
        protected readonly Mapper mapper = new();

        public abstract List<TEntityDto> Get();
        public abstract Task<List<TEntityDto>> GetAsync();

        public abstract TEntityDto? GetById(int id);
        public abstract Task<TEntityDto?> GetByIdAsync(int id);

        public abstract bool Save(TEntityDto entityDto);
        public abstract Task<bool> SaveAsync(TEntityDto entityDto);

        public abstract bool Update(TEntityDto entityDto);
        public abstract Task<bool> UpdateAsync(TEntityDto entityDto);

        public abstract bool Delete(int id);
        public abstract Task<bool> DeleteAsync(int id);

    }
}
