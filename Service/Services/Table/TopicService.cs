using AutoMapper;
using Common.Dto;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Service.Services.Search;

namespace Service.Services.Table
{
    public class TopicService : IService<TopicDto>, IOwner
    {
        private readonly IRepository<Topic> repository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SemanticSearchService _searchService;
        public TopicService(IRepository<Topic> repository, IMapper mapper, IHttpContextAccessor httpContextAccessor, SemanticSearchService searchService)
        {
            this.repository = repository;
            this.mapper = mapper;
            httpContextAccessor = httpContextAccessor;
            _searchService = searchService;
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

        public async Task<List<TopicDto>> SearchSimilarTopicsAndMessages(string text)
        {
            // שלב 1: הפוך את המחרוזת לווקטור
            var embedding = await _searchService.GetEmbeddingFromOpenAI(text);

            // שלב 2: שלח ל-Pinecone וחפש דומים
            var similarIds = await _searchService.SearchSimilarItems(embedding, topK: 5); // החזיר מזהים

            // שלב 3: תסנן את הנושאים במסד לפי ה-IDs
            var allTopics = await repository.GetAll(); // מביא את כל הנושאים
            var matchingTopics = allTopics
                .Where(t => similarIds.Contains(t.Id.ToString()))
                .ToList();
            return mapper.Map<List<TopicDto>>(matchingTopics);
        }
    }
}
