using ApplicationServices.DTOs;
using Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationServices.Utilities;
using Data.Entities;
using System.Linq.Expressions;

namespace ApplicationServices.Implementations
{
    public class UserManagementService : BaseManagementService<UserDto>
    {
        AuthenticationManagementService authService = new();

        public override List<UserDto> Get()
        {
            var results = new List<UserDto>();

            using var unitOfWork = new UnitOfWork();

            var users = unitOfWork.UsersRepository.Get();
            foreach (var user in users)
            {
                if (user is not null)
                    results.Add(mapper.MapTo<UserDto>(user)!);
            }

            return results;
        }

        public override async Task<List<UserDto>> GetAsync()
        {
            var results = new List<UserDto>();

            using var unitOfWork = new UnitOfWork();

            var users = await unitOfWork.UsersRepository.GetAsync();
            foreach (var user in users)
            {
                if (user is not null)
                    results.Add(mapper.MapTo<UserDto>(user)!);
            }

            return results;
        }

        public async Task<List<UserDto>> GetFilteredAsync(Expression<Func<User, bool>> filter)
        {
            var results = new List<UserDto>();

            using var unitOfWork = new UnitOfWork();

            var users = await unitOfWork.UsersRepository.GetAsync(filter);
            foreach (var user in users)
            {
                if (user is not null)
                    results.Add(mapper.MapTo<UserDto>(user)!);
            }

            return results;
        }

        public List<UserDto> GetPaged(int pageSize, int page)
        {
            var results = new List<UserDto>();

            using var unitOfWork = new UnitOfWork();

            var users = unitOfWork.UsersRepository.GetPaged(
                pageSize, page
            );

            foreach (var user in users)
            {
                if (user is not null)
                    results.Add(mapper.MapTo<UserDto>(user)!);
            }

            return results;
        }

        public async Task<List<UserDto>> GetPagedAsync(int pageSize, int page)
        {
            var results = new List<UserDto>();

            using var unitOfWork = new UnitOfWork();

            var users = await unitOfWork.UsersRepository.GetPagedAsync(
                pageSize, page
            );

            foreach (var user in users)
            {
                if (user is not null)
                    results.Add(mapper.MapTo<UserDto>(user)!);
            }

            return results;
        }

        public override UserDto? GetById(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserDto>(unitOfWork.UsersRepository.GetById(id));
        }

        public override async Task<UserDto?> GetByIdAsync(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<UserDto>(await unitOfWork.UsersRepository.GetByIdAsync(id));
        }

        public override bool Save(UserDto userDto)
        {
            throw new NotSupportedException();
            return false;
        }

        public override async Task<bool> SaveAsync(UserDto userDto)
        {
            throw new NotSupportedException();
            return false;
        }

        public override bool Update(UserDto userDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userDto)!;
            user.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(UserDto userDto)
        {
            using var unitOfWork = new UnitOfWork();

            var user = mapper.MapTo<User>(userDto)!;
            user.UpdatedOn = DateTime.UtcNow;

            var authCredUser = await authService.GetByIdAsync(userDto.Id);

            if (authCredUser is null) return false;

            user.Password = authCredUser.Password;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Delete(int id)
        {
            if (id == 1) return false; // user with id 1 is ultimate admin

            using var unitOfWork = new UnitOfWork();

            var messages = unitOfWork.MessagesRepository.Get(m => m.CreatedById == id);
            var messageBoards =  unitOfWork.MessageBoardsRepository.Get(m => m.CreatedById == id);

            try
            {
                foreach (var message in messages) unitOfWork.MessagesRepository.Delete(message.Id);
                foreach (var messageBoard in messageBoards) unitOfWork.MessagesRepository.Delete(messageBoard.Id);

                unitOfWork.UsersRepository.Delete(id);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            if (id == 1) return false; // user with id 1 is ultimate admin

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetAsync(m => m.CreatedById == id);
            var messageBoards = await unitOfWork.MessageBoardsRepository.GetAsync(m => m.CreatedById == id);

            try
            {
                foreach(var message in messages) unitOfWork.MessagesRepository.Delete(message.Id);
                foreach(var messageBoard in messageBoards) unitOfWork.MessagesRepository.Delete(messageBoard.Id);

                unitOfWork.UsersRepository.Delete(id);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> PromoteAdminAsync(int id)
        {
            using var unitOfWork = new UnitOfWork();

            var user = await unitOfWork.UsersRepository.GetByIdAsync(id);
            if (user is null || user.IsAdmin) return false;

            user.IsAdmin = true;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DemoteAdminAsync(int id)
        {
            if (id == 1) return false; // user with id 1 is ultimate admin

            using var unitOfWork = new UnitOfWork();

            var user = await unitOfWork.UsersRepository.GetByIdAsync(id);
            if (user is null || !user.IsAdmin) return false;

            user.IsAdmin = false;

            try
            {
                unitOfWork.UsersRepository.Update(user);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
