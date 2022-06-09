using ApplicationServices.DTOs;
using Data.Entities;

namespace ApplicationServices.Utilities
{
    public class Mapper
    {
        private Func<User, UserRegisterDto> userToRegisterDtoMap = user =>
        {
            return new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = user.Password
            };
        };
        private Func<UserRegisterDto, User> registerDtoToUserMap = dto =>
        {
            return new()
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Password = dto.Password
            };
        };
        private Func<User, UserAuthDto> userToAuthDtoMap = user =>
        {
            return new()
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password
            };
        };
        private Func<UserAuthDto, User> authDtoToUserMap = dto =>
        {
            return new()
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Password = dto.Password
            };
        };
        private Func<User, UserDto> userToDtoMap = user =>
        {
            return new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                IsAdmin = user.IsAdmin
            };
        };
        private Func<UserDto, User> dtoToUserMap = dto =>
        {
            return new()
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                IsAdmin = dto.IsAdmin
            };
        };

        private Func<Message, MessageDto> messageToDtoMap;
        private Func<MessageDto, Message> dtoToMessageMap;

        private Func<MessageBoard, MessageBoardDto> messageBoardToDtoMap;
        private Func<MessageBoardDto, MessageBoard> dtoToMessageBoardMap;

        public Mapper()
        {
            messageToDtoMap = message =>
            {
                return new()
                {
                    Id = message.Id,
                    Title = message.Title,
                    Content = message.Content,
                    IsImportant = message.IsImportant,

                    MessageBoardId = message.MessageBoardId,

                    CreatedById = message.CreatedById!.Value,
                    CreatedOn = message.CreatedOn!.Value
                };
            };

            dtoToMessageMap = dto =>
            {
                return new()
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Content = dto.Content,
                    IsImportant = dto.IsImportant,

                    MessageBoardId = dto.MessageBoardId,

                    CreatedById = dto.CreatedById,
                    CreatedOn = dto.CreatedOn
                };
            };

            messageBoardToDtoMap = messageBoard =>
            {
                return new()
                {
                    Id = messageBoard.Id,
                    Name = messageBoard.Name,
                    IsOpen = messageBoard.IsOpen,

                    CreatedById = messageBoard.CreatedById!.Value,
                    CreatedOn = messageBoard.CreatedOn!.Value,

                    UpdatedById = messageBoard.UpdatedById!.Value,
                    UpdatedOn = messageBoard.UpdatedOn!.Value
                };
            };

            dtoToMessageBoardMap = dto =>
            {
                return new()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    IsOpen = dto.IsOpen,

                    CreatedById = dto.CreatedById,
                    CreatedOn = dto.CreatedOn,

                    UpdatedById = dto.UpdatedById,
                    UpdatedOn = dto.UpdatedOn
                };
            };
        }

        public TEntityDto? MapTo<TEntityDto>(BaseEntity? entity)
            where TEntityDto : BaseDto
        {
            if (entity is null) return null;

            if (entity is User u)
            {
                if (typeof(TEntityDto) == typeof(UserDto)) return (TEntityDto)(BaseDto)userToDtoMap(u);
                else if (typeof(TEntityDto) == typeof(UserAuthDto)) return (TEntityDto)(BaseDto)userToAuthDtoMap(u);
                else if (typeof(TEntityDto) == typeof(UserRegisterDto)) return (TEntityDto)(BaseDto)userToAuthDtoMap(u);

                throw new NotSupportedException("Not supported entity dto type!");
            }
            
            if (entity is Message m) return (TEntityDto)(BaseDto)messageToDtoMap(m);
            
            if (entity is MessageBoard mb) return (TEntityDto)(BaseDto)messageBoardToDtoMap(mb);

            throw new NotSupportedException("Not supported entity type!");
        }

        public TEntity? MapTo<TEntity>(BaseDto? dto)
            
            where TEntity : BaseEntity
        {
            if (dto is null) return null;

            if (dto is UserDto u) return (TEntity)(BaseEntity)dtoToUserMap(u);

            if (dto is UserAuthDto ua) return (TEntity)(BaseEntity)authDtoToUserMap(ua);

            if (dto is UserRegisterDto ur) return (TEntity)(BaseEntity)registerDtoToUserMap(ur);

            if (dto is MessageDto m) return (TEntity)(BaseEntity)dtoToMessageMap(m);
            
            if (dto is MessageBoardDto mb) return (TEntity)(BaseEntity)dtoToMessageBoardMap(mb);

            throw new NotSupportedException("Not supported entity dto type!");
        }
    }
}