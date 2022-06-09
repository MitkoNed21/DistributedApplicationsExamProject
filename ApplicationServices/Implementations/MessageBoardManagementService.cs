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
    public class MessageBoardManagementService : BaseManagementService<MessageBoardDto>
    {
        public override List<MessageBoardDto> Get()
        {
            var results = new List<MessageBoardDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoards = unitOfWork.MessageBoardsRepository.Get(
                orderBy: x => x.OrderByDescending(mb => mb.UpdatedOn)
            );
            foreach (var messageBoard in messageBoards)
            {
                if (messageBoard is not null)
                    results.Add(mapper.MapTo<MessageBoardDto>(messageBoard)!);
            }

            return results;
        }

        public override async Task<List<MessageBoardDto>> GetAsync()
        {
            var results = new List<MessageBoardDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoards = await unitOfWork.MessageBoardsRepository.GetAsync(/*includeProperties: "CreatedBy,UpdatedBy"*/
                orderBy: x => x.OrderByDescending(mb => mb.UpdatedOn)
            );
            foreach (var messageBoard in messageBoards)
            {
                if (messageBoard is not null)
                    results.Add(mapper.MapTo<MessageBoardDto>(messageBoard)!);
            }

            return results;
        }

        public async Task<List<MessageBoardDto>> GetFilteredAsync(Expression<Func<MessageBoard, bool>> filter)
        {
            var results = new List<MessageBoardDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoards = await unitOfWork.MessageBoardsRepository.GetAsync(filter);
            foreach (var messageBoard in messageBoards)
            {
                if (messageBoard is not null)
                    results.Add(mapper.MapTo<MessageBoardDto>(messageBoard)!);
            }

            return results;
        }

        public List<MessageBoardDto> GetPaged(int pageSize, int page)
        {
            var results = new List<MessageBoardDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoards = unitOfWork.MessageBoardsRepository.GetPaged(
                pageSize, page,
                orderBy: x => x.OrderBy(mb => mb.UpdatedOn)
            );

            foreach (var messageBoard in messageBoards)
            {
                if (messageBoard is not null)
                    results.Add(mapper.MapTo<MessageBoardDto>(messageBoard)!);
            }

            return results;
        }

        public async Task<List<MessageBoardDto>> GetPagedAsync(int pageSize, int page)
        {
            var results = new List<MessageBoardDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoards = await unitOfWork.MessageBoardsRepository.GetPagedAsync(
                pageSize, page,
                orderBy: x => x.OrderBy(mb => mb.UpdatedOn)
            );

            foreach (var messageBoard in messageBoards)
            {
                if (messageBoard is not null)
                    results.Add(mapper.MapTo<MessageBoardDto>(messageBoard)!);
            }

            return results;
        }

        public override MessageBoardDto? GetById(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<MessageBoardDto>(unitOfWork.MessageBoardsRepository.GetById(id));
        }

        public override async Task<MessageBoardDto?> GetByIdAsync(int id)
        {
            using var unitOfWork = new UnitOfWork();

            return mapper.MapTo<MessageBoardDto>(await unitOfWork.MessageBoardsRepository.GetByIdAsync(id));
        }

        public override bool Save(MessageBoardDto messageBoardDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoard = mapper.MapTo<MessageBoard>(messageBoardDto)!;
            messageBoard.CreatedOn = DateTime.UtcNow;
            messageBoard.UpdatedOn = messageBoard.CreatedOn;

            try
            {
                unitOfWork.MessageBoardsRepository.Insert(messageBoard);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> SaveAsync(MessageBoardDto messageBoardDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoard = mapper.MapTo<MessageBoard>(messageBoardDto)!;
            messageBoard.CreatedOn = DateTime.UtcNow;
            messageBoard.UpdatedOn = messageBoard.CreatedOn;

            try
            {
                await unitOfWork.MessageBoardsRepository.InsertAsync(messageBoard);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool Update(MessageBoardDto messageBoardDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoard = mapper.MapTo<MessageBoard>(messageBoardDto)!;
            messageBoard.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessageBoardsRepository.Update(messageBoard);
                unitOfWork.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> UpdateAsync(MessageBoardDto messageBoardDto)
        {
            using var unitOfWork = new UnitOfWork();

            var messageBoard = mapper.MapTo<MessageBoard>(messageBoardDto)!;
            messageBoard.UpdatedOn = DateTime.UtcNow;

            try
            {
                unitOfWork.MessageBoardsRepository.Update(messageBoard);
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

            try
            {
                unitOfWork.MessageBoardsRepository.Delete(id);
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

            try
            {
                unitOfWork.MessageBoardsRepository.Delete(id);
                await unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<MessageDto>> GetMessagesAsync(int messageBoardId)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetAsync(
                m => m.MessageBoardId == messageBoardId
            );

            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public async Task<List<MessageDto>> GetMessagesFilteredAsync(int messageBoardId, Expression<Func<Message, bool>> filter)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messageBoard = (await unitOfWork.MessageBoardsRepository.GetAsync(
                mb => mb.Id == messageBoardId,
                includeProperties: nameof(MessageBoard.Messages)
            )).FirstOrDefault();

            if (messageBoard is null) return new();

            var messages = messageBoard.Messages.Where(filter.Compile());

            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }

        public async Task<List<MessageDto>> GetMessagesPagedAsync(int messageBoardId, int pageSize, int page)
        {
            var results = new List<MessageDto>();

            using var unitOfWork = new UnitOfWork();

            var messages = await unitOfWork.MessagesRepository.GetPagedAsync(
                pageSize, page,
                m => m.MessageBoardId == messageBoardId
            );

            foreach (var message in messages)
            {
                if (message is not null)
                    results.Add(mapper.MapTo<MessageDto>(message)!);
            }

            return results;
        }
    }
}
