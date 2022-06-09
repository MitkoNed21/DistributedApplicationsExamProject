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
    public class MessageManagementService : BaseManagementService<MessageDto>
    {
        public override List<MessageDto> Get()
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = unitOfWork.MessagesRepository.Get(
                orderBy: x => x.OrderByDescending(m => m.IsImportant)
                               .ThenBy(m => m.CreatedOn)
            );

            foreach (var message in messages)
            {
                results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public override async Task<List<MessageDto>> GetAsync()
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetAsync(
                orderBy: x => x.OrderByDescending(m => m.IsImportant)
                               .ThenBy(m => m.CreatedOn)
            );
            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public async Task<List<MessageDto>> GetFilteredAsync(Expression<Func<Message, bool>> filter)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetAsync(filter);
            foreach (var meassage in messages)
            {
                if (meassage is not null)
                    results.Add(mapper.MapTo<MessageDto>(meassage)!);
            }

            return results;
        }

        public List<MessageDto> GetPaged(int pageSize, int page)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = unitOfWork.MessagesRepository.GetPaged(
                pageSize, page,
                orderBy: x => x.OrderByDescending(m => m.IsImportant)
                               .ThenBy(m => m.CreatedOn)
            );

            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public async Task<List<MessageDto>> GetPagedAsync(int pageSize, int page)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetPagedAsync(
                pageSize, page,
                orderBy: x => x.OrderByDescending(m => m.IsImportant)
                               .ThenBy(m => m.CreatedOn)
            );

            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public override MessageDto? GetById(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<MessageDto>(unitOfWork.MessagesRepository.GetById(id));
        }

        public override async Task<MessageDto?> GetByIdAsync(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<MessageDto>(await unitOfWork.MessagesRepository.GetByIdAsync(id));
        }

        public override bool Save(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage = unitOfWork.MessageBoardsRepository.GetById(messageDto.MessageBoardId);

            if (messageBoardOfMessage is null || !messageBoardOfMessage.IsOpen) return false;

            var message = mapper.MapTo<Message>(messageDto)!;
            message.CreatedOn = DateTime.UtcNow;
            messageBoardOfMessage.UpdatedOn = message.UpdatedOn = message.CreatedOn;

            try
            {
                unitOfWork.MessagesRepository.Insert(message);
                unitOfWork.MessageBoardsRepository.Update(messageBoardOfMessage);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CanBePosted(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage = unitOfWork.MessageBoardsRepository.GetById(messageDto.MessageBoardId);

            if (messageBoardOfMessage is not null && messageBoardOfMessage.IsOpen)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CanBePostedAsync(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage = await unitOfWork.MessageBoardsRepository.GetByIdAsync(messageDto.MessageBoardId);

            if (messageBoardOfMessage is not null && messageBoardOfMessage.IsOpen)
            {
                return true;
            }

            return false;
        }

        public override async Task<bool> SaveAsync(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage = 
                await unitOfWork.MessageBoardsRepository.GetByIdAsync(messageDto.MessageBoardId);

            if (messageBoardOfMessage is null || !messageBoardOfMessage.IsOpen) return false;

            var message = mapper.MapTo<Message>(messageDto)!;
            message.CreatedOn = DateTime.UtcNow;
            messageBoardOfMessage.UpdatedOn = message.UpdatedOn = message.CreatedOn;

            try
            {
                await unitOfWork.MessagesRepository.InsertAsync(message);
                unitOfWork.MessageBoardsRepository.Update(messageBoardOfMessage);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Update(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage =
                unitOfWork.MessageBoardsRepository.GetById(messageDto.MessageBoardId);

            if (messageBoardOfMessage is null || !messageBoardOfMessage.IsOpen) return false;

            var message = mapper.MapTo<Message>(messageDto)!;
            messageBoardOfMessage.UpdatedOn = message.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessagesRepository.Update(message);
                unitOfWork.MessageBoardsRepository.Update(messageBoardOfMessage);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(MessageDto messageDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoardOfMessage =
                await unitOfWork.MessageBoardsRepository.GetByIdAsync(messageDto.MessageBoardId);

            if (messageBoardOfMessage is null || !messageBoardOfMessage.IsOpen) return false;

            var message = mapper.MapTo<Message>(messageDto)!;
            messageBoardOfMessage.UpdatedOn = message.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessagesRepository.Update(message);
                unitOfWork.MessageBoardsRepository.Update(messageBoardOfMessage);
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
            using var unitOfWork = new UnitOfWork();

            var message = unitOfWork.MessagesRepository.Get(
                m => m.Id == id,
                includeProperties: nameof(Message.MessageBoard)
            ).FirstOrDefault();

            if (message is null || !message.MessageBoard.IsOpen) return false;

            message.MessageBoard.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessagesRepository.Delete(message);
                unitOfWork.MessageBoardsRepository.Update(message.MessageBoard);
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
            using var unitOfWork = new UnitOfWork();

            var message = (await unitOfWork.MessagesRepository.GetAsync(
                m => m.Id == id,
                includeProperties: nameof(Message.MessageBoard)
            )).FirstOrDefault();

            if (message is null || !message.MessageBoard.IsOpen) return false;

            message.MessageBoard.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessagesRepository.Delete(message);
                unitOfWork.MessageBoardsRepository.Update(message.MessageBoard);
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
