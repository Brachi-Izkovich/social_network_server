using AutoMapper;
using Common.Dto;
using Repository.Entities;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRepository<Topic> _topicRepo;
        private readonly IRepository<Message> _messageRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public SearchService(IRepository<Topic> topicRepo, IRepository<Message> messageRepo, IRepository<User> userRepo, IMapper mapper)
        {
            _topicRepo = topicRepo;
            _messageRepo = messageRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<List<SearchResultDto>> SearchSmart(string query)
        {
            var cleanedQueryWords = ExtractCleanWords(query);

            var allTopics = _topicRepo.GetAll();
            var allMessages = _messageRepo.GetAll();

            var results = new List<SearchResultDto>();
            foreach (var topic in allTopics.Result)
            {
                // the mess that has this topicid
                var topicMessages = allMessages.Result.Where(m => m.TopicId == topic.Id).ToList();

                //compare dimyon between the query and this topic title
                var matchScore = GetSimilarity(cleanedQueryWords, ExtractCleanWords(topic.Title));

                //
                var matchingMessage = topicMessages.FirstOrDefault(m => GetSimilarity(cleanedQueryWords, ExtractCleanWords(m.Content)) > 0.3);

                if (matchScore > 0.3 || matchingMessage != null)
                {
                    var dto = _mapper.Map<SearchResultDto>(topic);

                    if (matchingMessage != null)
                    {
                        dto.MatchingMessageId = matchingMessage.Id;
                        dto.MatchingMessageSnippet = matchingMessage.Content;
                    }
                    results.Add(dto);
                }
            }
            //return like the time creating
            return await Task.FromResult(results.OrderByDescending(t => t.CreatedAt).ToList());
        }

        private static readonly HashSet<string> stopWords = new()
        {
            // כינויי גוף
            "אני", "אתה", "את", "הוא", "היא", "אנחנו", "אתם", "אתן", "הם", "הן", "שלי", "שלך", "שלו", "שלה", "שלנו", "שלהם", "שלהן",

            // פעלים כלליים
            "רוצה", "רוצים", "רוצה", "רוצות", "צריך", "צריכה", "צריך", "חושב", "חושבת", "חושבים", "צריך", "יש", "אין", "היה", "היו", "תהיה", "תהיו",

            // מילות שאלה/פנייה
            "מה", "מי", "איפה", "מתי", "איך", "למה", "כמה", "אם", "אז", "כן", "לא", "האם",

            // מילות יחס / קישור
            "עם", "על", "אל", "את", "של", "אתה", "ב", "ל", "ו", "ה", "מ", "כ", "ש", "ועם", "ואת", "ואל", "אלי", "בי", "לי", "לך", "לו", "לה", "לנו", "להם", "להן",

            // מילות נימוס או מילים כלליות
            "שלום", "בבקשה", "תודה", "סליחה", "היי", "הי", "שלום", "אפשר", "מישהו", "מישהי", "משהו", "כלום", "אחד", "אחת", "דבר", "דברים", "כל", "עוד", "כמו", "לפי", "אולי", "עכשיו", "כאן", "שם", "תוך", "בזמן", "גם", "רק",

            // מילות עזר נוספות
            "שהוא", "שהיא", "שאתה", "שאת", "שאנחנו", "שהם", "שיש", "שאין", "שזה", "כדי", "יותר", "פחות", "מאוד", "גם", "רק", "כבר", "עוד", "מעט", "הרבה", "מעטים", "אחר", "אחרים", "אחרת"
        };
        private static readonly HashSet<string> keywords = new()
        {
            // מושגים חשובים כלליים
            "שיר", "חומר", "בעיה", "פתרון", "שאלה", "תשובה", "מדריך", "הסבר", "עזרה", "עוזר", "תוצאה", "פתרון", "שגיאה", "טעות", "תקלה", "בקשה", "דיון", "נושא",
            // 👨‍🏫 למידה
            "ללמוד", "לימוד", "שיעור", "מורה", "תרגיל", "עבודה", "פרויקט", "מבחן", "בחינה", "תשובות", "מטלה", "סיכום",
            
            // 💻 תכנות (שפות ותכנים)
            "קוד", "תכנות", "פיתוח", "תוכנה", "באג", "באג", "דיבאג", "קומפילציה", "אלגוריתם", "מחלקה", "פונקציה", "משתנה", "לולאה", "תנאי", "אובייקט",
            
            // 🧾 שפות תכנות נפוצות
            "c#", "c++", "java", "python", "javascript", "typescript", "sql", "html", "css", "react", "angular", "node", "php", "kotlin", "swift", "go", "bash", "shell", "json", "xml",
            
            // 🔍 כלים וטכנולוגיות
            ".net", "entity", "framework", "linq", "visual", "studio", "git", "github", "api", "rest", "postman", "swagger", "docker", "azure", "firebase", "server", "client",
            
            // 🗂 בסיסי נתונים
            "sql", "nosql", "mysql", "mongodb", "postgres", "sqlite", "db", "database", "שאילתה", "query", "טבלה", "עמודה", "שדה", "מפתח", "index", "foreign", "key", "primary",
            
            // 📚 כללי עזרה
            "שמוליק", "שיתוף", "שליחה", "לשלוח", "מסמך", "קובץ", "קישור", "לינק", "המלצה", "מומלץ", "חובה", "תודה", "עזרתם"

        };

        private HashSet<string> ExtractCleanWords(string input) =>
            input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => NormalizeHebrewWord(w.Trim(',', '.', '?', '!', '"', ':', ';')))
            .Where(w => !stopWords.Contains(w))
            .ToHashSet();

        private double GetSimilarity(HashSet<string> a, HashSet<string> b)
        {
            if (a.Count == 0 || b.Count == 0) return 0;
            double score = 0;
            //counting the common words in a and b
            foreach (string word in a.Intersect(b))
            {
                score += keywords.Contains(word) ? 2.0 : 1.0;
            }
            return score / a.Union(b).Count();
        }

        //לא קראתי עדיין!!!!!!!!!!!!! להבין!!!!
        private string NormalizeHebrewWord(string word)
        {
            if (word.StartsWith("ל") && word.Length > 3) word = word.Substring(1); // להסיר "ל" (כמו לכתוב)
            if (word.EndsWith("ים") || word.EndsWith("ות")) word = word.Substring(0, word.Length - 2); // רבים
            if (word.EndsWith("ה") || word.EndsWith("ת") || word.EndsWith("ן") || word.EndsWith("ך")) word = word.Substring(0, word.Length - 1); // סיומות נקבה או נסתר
            return word;
        }

    }
}
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
