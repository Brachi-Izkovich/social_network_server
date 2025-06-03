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
    public class FeedbackService : IService<FeedbackDto>
    {
        private readonly IRepository<Feedback> repository;
        private readonly IMapper mapper;
        public FeedbackService(IRepository<Feedback> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<FeedbackDto> Add(FeedbackDto user)
        {
            return mapper.Map<FeedbackDto>(await repository.Add(mapper.Map<Feedback>(user)));
        }

        public async Task Delete(int id)
        {
            await repository.Delete(id);
        }

        public async Task<List<FeedbackDto>> GetAll()
        {
            return mapper.Map<List<FeedbackDto>>(await repository.GetAll());
        }

        public async Task<FeedbackDto> GetById(int id)
        {
            return mapper.Map<FeedbackDto>(await repository.GetById(id));
        }

        public async Task Update(int id, FeedbackDto item)
        {
            await repository.Update(id, mapper.Map<Feedback>(item));
        }
    }
}
