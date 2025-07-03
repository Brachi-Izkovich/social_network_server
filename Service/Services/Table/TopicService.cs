using AutoMapper;
using Common.Dto;
using Microsoft.AspNetCore.Http;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Table
{
    public class TopicService : IOwner, ITopicService
    {
        private readonly IRepository<Topic> repository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Topic> _topicRepository;

        public TopicService(IRepository<Topic> repository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IRepository<Topic> topicRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            httpContextAccessor = httpContextAccessor;
            _topicRepository = topicRepository;
        }

        public async Task<TopicDto> Add(TopicDto topicDto)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                throw new UnauthorizedAccessException("User ID not found in token.");

            var topic = new Topic()
            {
                Title = topicDto.Title,
                UserId = userId,
                CategoryId = topicDto.CategoryId,
                ListMessages = new List<Message>()
            };
            var addedTopic = await repository.Add(topic);
            return mapper.Map<TopicDto>(addedTopic);
        }

        public async Task Delete(int id)
        {
            await repository.Delete(id);
        }

        public async Task<List<TopicDto>> GetAll()
        {
            return mapper.Map<List<TopicDto>>(await repository.GetAll());
        }

        public async Task<TopicDto> GetById(int id)
        {
            return mapper.Map<TopicDto>(await repository.GetById(id));
        }

        public async Task<bool> IsOwner(int topicId, int userId)
        {
            var feedback = await repository.GetById(topicId);
            return feedback != null && feedback.UserId == userId;
        }

        public async Task Update(int id, TopicDto item)
        {
            await repository.Update(id, mapper.Map<Topic>(item));
        }

        // Search

        public async Task<List<TopicDto>> GetSimilarTopicsAsync(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return new List<TopicDto>();

            string Normalize(string text) =>
                text.ToLower().Trim();

            var normalizedNew = Normalize(newTitle);

            var allTopics = await repository.GetAll();

            var similar = allTopics
                .Where(t => GetSimilarityScore(Normalize(t.Title), normalizedNew) > 0.4)
                .ToList();

            return mapper.Map<List<TopicDto>>(similar);
        }

        private double GetSimilarityScore(string s1, string s2)
        {
            var words1 = s1.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            var words2 = s2.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

            if (words1.Count == 0 || words2.Count == 0)
                return 0;

            var intersection = words1.Intersect(words2).Count();
            var union = words1.Union(words2).Count();

            return (double)intersection / union;
        }
    }
}
