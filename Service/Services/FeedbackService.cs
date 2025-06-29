using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FeedbackService : IService<FeedbackDto>,IOwner
    {
        private readonly IRepository<Feedback> repository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FeedbackService(IRepository<Feedback> repository, IMapper mapper, IHttpContextAccessor _httpContextAccessor)
        {
            this.repository = repository;
            this.mapper = mapper;
            this._httpContextAccessor = _httpContextAccessor;
        }
        public async Task<FeedbackDto> Add(FeedbackDto feedbackDto)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var feedback = new Feedback()
            {
               Type=feedbackDto.Type,
               UserId=userId,
               MessageId=feedbackDto.MessageId
            };
            var addedFeedback = await repository.Add(feedback);
            return mapper.Map<FeedbackDto>(addedFeedback);
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

        public async Task<bool> IsOwner(int feedbackId, int userId)
        {
            var feedback = await repository.GetById(feedbackId);
            return feedback != null && feedback.UserId == userId;
        }
        public async Task Update(int id, FeedbackDto item)
        {
            await repository.Update(id, mapper.Map<Feedback>(item));
        }
    }
}
