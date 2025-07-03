//using Common.Dto;
//using Repository.Entities;
//using Repository.Interfaces;
//using Service.Interfaces;

//namespace Service.Services
//{
//    public class SearchService : ISearchService
//    {
//        private readonly IRepository<Topic> _topicRepo;
//        private readonly IRepository<Message> _messageRepo;
//        private readonly IRepository<User> _userRepo;

//        public SearchService(
//            IRepository<Topic> topicRepo,
//            IRepository<Message> messageRepo,
//            IRepository<User> userRepo)
//        {
//            _topicRepo = topicRepo;
//            _messageRepo = messageRepo;
//            _userRepo = userRepo;
//        }

//        public async Task<List<SearchResultDto>> SearchSmart(string query)
//        {
//            var keywords = ExtractCleanWords(query);
//            var results = new List<SearchResultDto>();

//            var topics = await _topicRepo.GetAll();
//            var messages = await _messageRepo.GetAll();

//            foreach (var topic in topics)
//            {
//                var score = GetSmartSimilarity(keywords, ExtractCleanWords(topic.Title));
//                if (score > 0.3)
//                {
//                    results.Add(new SearchResultDto
//                    {
//                        Type = "Topic",
//                        Id = topic.Id,
//                        TitleOrContent = topic.Title,
//                        AuthorName = await GetUserName(topic.UserId),
//                        CreatedAt = null // אין בטופיק CreatedAt
//                    });
//                }
//            }

//            foreach (var msg in messages)
//            {
//                var score = GetSmartSimilarity(keywords, ExtractCleanWords(msg.Content));
//                if (score > 0.3)
//                {
//                    results.Add(new SearchResultDto
//                    {
//                        Type = "Message",
//                        Id = msg.Id,
//                        TitleOrContent = msg.Content,
//                        AuthorName = await GetUserName(msg.UserId),
//                        CreatedAt = msg.TimeSend
//                    });
//                }
//            }

//            // ממיין לפי CreatedAt אם קיים, אחרת לפי Id
//            return results
//                .OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue)
//                .ThenByDescending(r => r.Id)
//                .ToList();
//        }

//        private async Task<string> GetUserName(int? userId)
//        {
//            if (userId == null) return "Unknown";
//            var user = await _userRepo.GetById(userId.Value);
//            return user?.Name ?? "Unknown";
//        }

//        // מילות עצירה שלא חשובות בחיפוש
//        private static readonly HashSet<string> stopWords = new()
//        {
//            "אני", "יש", "רוצה", "בבקשה", "ל", "ב", "ו", "ה", "צריך", "צריכה", "מתי", "לשלוח", "כמה", "שיותר", "מהר", "האם"
//        };

//        // מילות מפתח שיקבלו ניקוד כפול
//        private static readonly HashSet<string> keywords = new()
//        {
//            "שיר", "c#", "חומר", "שמוליק", "בעיה", "תגובה"
//        };

//        private HashSet<string> ExtractCleanWords(string input) =>
//            input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries)
//                .Select(w => w.Trim(',', '.', '?', '!', ':', ';'))
//                .Where(w => !stopWords.Contains(w))
//                .ToHashSet();

//        private double GetSmartSimilarity(HashSet<string> a, HashSet<string> b)
//        {
//            if (a.Count == 0 || b.Count == 0) return 0;
//            double score = 0;
//            foreach (var word in a.Intersect(b))
//                score += keywords.Contains(word) ? 2.0 : 1.0;
//            return score / a.Union(b).Count();
//        }
//    }
//}
