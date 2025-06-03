using AutoMapper;
using Common.Dto;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MessageService : IService<MessageDto>
    {
        private readonly IRepository<Message> repository;
        private readonly IMapper mapper;
        public MessageService(IRepository<Message> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<MessageDto> Add(MessageDto user)
        {
            return mapper.Map<MessageDto>(await repository.Add(mapper.Map<Message>(user)));
        }

        public async Task Delete(int id)
        {
            await repository.Delete(id);
        }

        public async Task<List<MessageDto>> GetAll()
        {
            return mapper.Map<List<MessageDto>>(await repository.GetAll());
        }

        public async Task<MessageDto> GetById(int id)
        {
            return mapper.Map<MessageDto>(await repository.GetById(id));
        }

        public async Task Update(int id, MessageDto item)
        {
            await repository.Update(id, mapper.Map<Message>(item));
        }
    }
}
